using Microsoft.EntityFrameworkCore;
using SWA.Domain.Common;
using SWA.Domain.Content.Documents;
using SWA.Domain.Content.Events;
using SWA.Domain.Content.Faqs;
using SWA.Domain.Content.News;
using SWA.Domain.Content.Pages;
using SWA.Domain.Content.Services;
using SWA.Domain.Media;

namespace SWA.Infrastructure.Persistence;

/// <summary>Read-mostly content store — no ASP.NET Identity here, this app has no auth/user management.</summary>
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<PageTranslation> PageTranslations => Set<PageTranslation>();

    public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
    public DbSet<NewsArticleTranslation> NewsArticleTranslations => Set<NewsArticleTranslation>();

    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventTranslation> EventTranslations => Set<EventTranslation>();
    public DbSet<EventType> EventTypes => Set<EventType>();
    public DbSet<EventTypeTranslation> EventTypeTranslations => Set<EventTypeTranslation>();

    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<FaqTranslation> FaqTranslations => Set<FaqTranslation>();
    public DbSet<FaqCategory> FaqCategories => Set<FaqCategory>();
    public DbSet<FaqCategoryTranslation> FaqCategoryTranslations => Set<FaqCategoryTranslation>();

    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceTranslation> ServiceTranslations => Set<ServiceTranslation>();
    public DbSet<ServiceAudience> ServiceAudiences => Set<ServiceAudience>();
    public DbSet<ServiceAudienceTranslation> ServiceAudienceTranslations => Set<ServiceAudienceTranslation>();
    public DbSet<ServiceActivityType> ServiceActivityTypes => Set<ServiceActivityType>();
    public DbSet<ServiceActivityTypeTranslation> ServiceActivityTypeTranslations => Set<ServiceActivityTypeTranslation>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<ServiceCategoryTranslation> ServiceCategoryTranslations => Set<ServiceCategoryTranslation>();
    public DbSet<ServiceChannel> ServiceChannels => Set<ServiceChannel>();
    public DbSet<ServiceChannelTranslation> ServiceChannelTranslations => Set<ServiceChannelTranslation>();

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentTranslation> DocumentTranslations => Set<DocumentTranslation>();
    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();
    public DbSet<DocumentCategoryTranslation> DocumentCategoryTranslations => Set<DocumentCategoryTranslation>();

    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
