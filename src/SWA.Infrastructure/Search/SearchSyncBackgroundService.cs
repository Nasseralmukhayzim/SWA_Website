using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Search;
using SWA.Domain.Content;
using SWA.Domain.Enums;
using SWA.Domain.Media;
using SWA.Infrastructure.Persistence;
using SWA.Infrastructure.Search.Mappers;

namespace SWA.Infrastructure.Search;

/// <summary>
/// Keeps the Elasticsearch index in sync with SQL Server by polling on an interval. This app is
/// read-only against a DB owned by a separate CMS app, so there is no "on save" hook to index
/// from — every content type is re-checked on a delta (UpdatedAtUtc-based) or full sweep.
/// </summary>
public sealed class SearchSyncBackgroundService(
    IServiceScopeFactory scopeFactory,
    ISearchIndexer indexer,
    ElasticsearchOptions options,
    IConfiguration configuration,
    ILogger<SearchSyncBackgroundService> logger) : BackgroundService
{
    private DateTime? _lastSyncUtc;
    private int _tickCount;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await indexer.EnsureIndexAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to ensure the search index exists on startup; will retry on the first sync tick.");
        }

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Math.Max(options.SyncIntervalSeconds, 5)));
        do
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Search sync tick failed.");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        await indexer.EnsureIndexAsync(cancellationToken);

        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var fullSweep = _lastSyncUtc is null || ++_tickCount % Math.Max(options.FullSweepEveryNTicks, 1) == 0;
        var since = fullSweep ? (DateTime?)null : _lastSyncUtc;
        var syncStartedAtUtc = DateTime.UtcNow;

        await SyncAsync(db.Pages.Include(p => p.Translations), since, PageSearchDocumentMapper.Map, "Page", cancellationToken);
        await SyncAsync(db.NewsArticles.Include(n => n.Translations), since, NewsArticleSearchDocumentMapper.Map, "NewsArticle", cancellationToken);
        await SyncAsync(db.Events.Include(e => e.Translations), since, EventSearchDocumentMapper.Map, "Event", cancellationToken);
        await SyncAsync(
            db.Faqs.Include(f => f.Translations).Include(f => f.Category!).ThenInclude(c => c.Translations),
            since, FaqSearchDocumentMapper.Map, "Faq", cancellationToken);
        await SyncAsync(
            db.Services
                .Include(s => s.Translations)
                .Include(s => s.ServiceCategory!).ThenInclude(c => c.Translations)
                .Include(s => s.ServiceActivityType!).ThenInclude(a => a.Translations),
            since, ServiceSearchDocumentMapper.Map, "Service", cancellationToken);
        var attachments = await BuildDocumentAttachmentsAsync(db, since, cancellationToken);
        await SyncAsync(
            db.Documents.Include(d => d.Translations).Include(d => d.Category!).ThenInclude(c => c.Translations),
            since, (document, lang) => DocumentSearchDocumentMapper.Map(document, lang, attachments), "Document", cancellationToken);

        _lastSyncUtc = syncStartedAtUtc;
    }

    /// <summary>
    /// Reads the on-disk files behind any Document translation touched this tick (PDF/Word only,
    /// under the configured size cap) and base64-encodes them for the attachment ingest pipeline.
    /// A missing or unreadable file just drops that one attachment — the document still indexes
    /// with its title/description, same fail-open spirit as the rest of this service.
    /// </summary>
    private async Task<Dictionary<Guid, string>> BuildDocumentAttachmentsAsync(
        ApplicationDbContext db, DateTime? since, CancellationToken cancellationToken)
    {
        var attachments = new Dictionary<Guid, string>();

        var mediaRoot = configuration["MediaStorage:RootPath"];
        if (string.IsNullOrWhiteSpace(mediaRoot))
        {
            return attachments;
        }

        var fileIds = await (since is null
                ? db.Documents
                : db.Documents.Where(d => d.UpdatedAtUtc == null || d.UpdatedAtUtc > since))
            .SelectMany(d => d.Translations)
            .Where(t => t.FileId != null)
            .Select(t => t.FileId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (fileIds.Count == 0)
        {
            return attachments;
        }

        var assets = await db.Set<MediaAsset>()
            .Where(a => fileIds.Contains(a.Id)
                && !a.IsDeleted
                && a.SizeInBytes <= options.MaxAttachmentSizeBytes
                && options.AttachmentContentTypes.Contains(a.ContentType))
            .ToListAsync(cancellationToken);

        foreach (var asset in assets)
        {
            var path = Path.Combine(mediaRoot, asset.StorageKey.Replace('/', Path.DirectorySeparatorChar));
            try
            {
                var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
                attachments[asset.Id] = Convert.ToBase64String(bytes);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                logger.LogWarning(ex, "Could not read attachment file {StorageKey} for media asset {MediaAssetId}; indexing without it.", asset.StorageKey, asset.Id);
            }
        }

        return attachments;
    }

    private async Task SyncAsync<TEntity>(
        IQueryable<TEntity> query,
        DateTime? since,
        Func<TEntity, string?, IEnumerable<SearchDocument>> map,
        string contentType,
        CancellationToken cancellationToken)
        where TEntity : PublishableContent
    {
        var rows = await (since is null ? query : query.Where(e => e.UpdatedAtUtc == null || e.UpdatedAtUtc > since))
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
        {
            return;
        }

        var toUpsert = new List<SearchDocument>();
        var toDelete = new List<Guid>();

        foreach (var row in rows)
        {
            if (SearchVisibility.IsPubliclyVisible(row))
            {
                toUpsert.AddRange(map(row, null));
            }
            else
            {
                toDelete.Add(row.Id);
            }
        }

        if (toUpsert.Count > 0)
        {
            await indexer.UpsertAsync(toUpsert, cancellationToken);
        }

        foreach (var entityId in toDelete)
        {
            await indexer.DeleteByEntityAsync(contentType, entityId, cancellationToken);
        }
    }
}

/// <summary>
/// Mirrors PublicContentQueryExtensions.PubliclyVisible, but for the indexer, which must always
/// index published-only content regardless of the public API's IncludeUnpublished dev toggle.
/// Kept as a standalone static so it's unit-testable without EF/ES.
/// </summary>
internal static class SearchVisibility
{
    public static bool IsPubliclyVisible(PublishableContent entity) =>
        !entity.IsDeleted && entity.DeletionStatus != DeletionRequestStatus.Approved && entity.Status == ContentStatus.Published;
}
