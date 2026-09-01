using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Search;
using AppSearchRequest = SWA.Application.Common.Interfaces.SearchRequest;

namespace SWA.Infrastructure.Search;

/// <summary>Elasticsearch-backed implementation of <see cref="ISearchIndexer"/>. One document per translation; see SearchDocument for the shared schema every content-type mapper produces.</summary>
public sealed class ElasticsearchIndexer(ElasticsearchClient client, ElasticsearchOptions options) : ISearchIndexer
{
    private const string AttachmentPipelineName = "swa-content-attachments";

    private readonly string _indexName = options.IndexName;

    public async Task EnsureIndexAsync(CancellationToken cancellationToken)
    {
        await EnsureAttachmentPipelineAsync(cancellationToken);

        var exists = await client.Indices.ExistsAsync(_indexName, cancellationToken);
        if (exists.Exists)
        {
            return;
        }

        var properties = new Properties
        {
            { "entityId", new KeywordProperty() },
            { "contentType", new KeywordProperty() },
            { "language", new KeywordProperty() },
            { "slug", new KeywordProperty() },
            {
                "title",
                new TextProperty
                {
                    Analyzer = "standard",
                    Fields = new Properties { { "ar", new TextProperty { Analyzer = "arabic" } } },
                }
            },
            {
                "body",
                new TextProperty
                {
                    Analyzer = "standard",
                    Fields = new Properties { { "ar", new TextProperty { Analyzer = "arabic" } } },
                }
            },
            { "taxonomyLabels", new TextProperty() },
            { "taxonomySlugs", new KeywordProperty() },
            { "updatedAtUtc", new DateProperty() },
            { "publishedAtUtc", new DateProperty() },
            { "sortOrder", new IntegerNumberProperty() },
        };

        await client.Indices.CreateAsync(_indexName, c => c.Mappings(new TypeMapping { Properties = properties }), cancellationToken);
    }

    /// <summary>
    /// Runs on every bulk upsert regardless of content type: the attachment processor is a no-op
    /// (ignore_missing) for the 5 content types that never set attachmentBase64, and picks up
    /// Documents' PDF/Word text today — Services' GuideFileId gets the same treatment for free
    /// later with only a mapper change, no pipeline change.
    /// </summary>
    private async Task EnsureAttachmentPipelineAsync(CancellationToken cancellationToken)
    {
        await client.Ingest.PutPipelineAsync(AttachmentPipelineName, r => r
            .Processors(
                new Processor
                {
                    Attachment = new AttachmentProcessor
                    {
                        Field = "attachmentBase64",
                        TargetField = "attachment",
                        IndexedChars = options.AttachmentIndexedChars,
                        IgnoreMissing = true,
                    },
                },
                new Processor
                {
                    Script = new ScriptProcessor
                    {
                        Source = "if (ctx.attachment != null && ctx.attachment.content != null) { ctx.body = ctx.body + ' | ' + ctx.attachment.content }",
                    },
                },
                new Processor
                {
                    Remove = new RemoveProcessor
                    {
                        Field = Fields.FromStrings(["attachmentBase64", "attachment"]),
                        IgnoreMissing = true,
                    },
                }),
            cancellationToken);
    }

    public async Task UpsertAsync(IReadOnlyCollection<SearchDocument> documents, CancellationToken cancellationToken)
    {
        if (documents.Count == 0)
        {
            return;
        }

        await client.BulkAsync(b => b
            .Index(_indexName)
            .Pipeline(AttachmentPipelineName)
            .IndexMany(documents, (descriptor, doc) => descriptor.Id(doc.Id)), cancellationToken);
    }

    public async Task DeleteByEntityAsync(string contentType, Guid entityId, CancellationToken cancellationToken)
    {
        await client.DeleteByQueryAsync<SearchDocument>(_indexName, d => d
            .Query(q => q
                .Bool(bo => bo
                    .Filter(
                        f => f.Term(t => t.Field("contentType").Value(contentType)),
                        f => f.Term(t => t.Field("entityId").Value(entityId.ToString()))))),
            cancellationToken);
    }

    public async Task<SearchResultPage> SearchAsync(AppSearchRequest request, CancellationToken cancellationToken)
    {
        var arabic = string.Equals(request.Lang, "ar", StringComparison.OrdinalIgnoreCase);
        var titleField = arabic ? "title.ar" : "title";
        var bodyField = arabic ? "body.ar" : "body";
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 20 : request.PageSize;
        var from = (page - 1) * pageSize;
        var hasContentTypeFilter = !string.IsNullOrWhiteSpace(request.ContentType);

        // Index has one document per translation; without this, an "ar" query could also match
        // an "en" document whose body happens to contain Arabic tokens (e.g. two translations of
        // the same Document sharing one attached file, so the extracted PDF text — analyzed via
        // body.ar regardless of the doc's own language — lands in both), producing a visible
        // duplicate (same slug/contentType) in the results.
        var languageValue = string.IsNullOrWhiteSpace(request.Lang) ? "en" : request.Lang.Trim().ToLowerInvariant();

        var response = await client.SearchAsync<SearchDocument>(s => s
            .Indices(_indexName)
            .From(from)
            .Size(pageSize)
            .Query(q => q.Bool(bo =>
            {
                bo.Must(m => m.MultiMatch(mm => mm
                    .Query(request.Query)
                    .Fields(new[] { $"{titleField}^3", bodyField })));

                if (hasContentTypeFilter)
                {
                    bo.Filter(
                        f => f.Term(t => t.Field("language").Value(languageValue)),
                        f => f.Term(t => t.Field("contentType").Value(request.ContentType!)));
                }
                else
                {
                    bo.Filter(f => f.Term(t => t.Field("language").Value(languageValue)));
                }
            }))
            .Highlight(h => h.Fields(f => f.Add(bodyField, hf => { })))
            .Sort(so => so.Score(sc => { }), so => so.Field("sortOrder", fs => { })), cancellationToken);

        var hits = response.Hits.Select(hit =>
        {
            var doc = hit.Source!;
            var snippet = hit.Highlight is not null && hit.Highlight.TryGetValue(bodyField, out var fragments) && fragments.Count > 0
                ? string.Join(" … ", fragments)
                : doc.Body.Length > 200 ? string.Concat(doc.Body.AsSpan(0, 200), "…") : doc.Body;

            return new SearchResultHit(doc.EntityId, doc.ContentType, doc.Slug, doc.Title, snippet, doc.TaxonomyLabels, doc.UpdatedAtUtc);
        }).ToList();

        return new SearchResultPage(hits, (int)response.Total);
    }
}
