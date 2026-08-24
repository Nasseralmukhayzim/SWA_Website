using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SWA.Domain.Content.News;
using SWA.Domain.Media;

namespace SWA.Infrastructure.Persistence.Configurations;

internal sealed class NewsArticleConfiguration : IEntityTypeConfiguration<NewsArticle>
{
    public void Configure(EntityTypeBuilder<NewsArticle> builder)
    {
        builder.ToTable("NewsArticles");
        builder.ConfigurePublishableContent();
        builder.Property(e => e.IsFeatured).IsRequired();
        builder.Property(e => e.ViewCount).IsRequired();

        builder.HasOne<MediaAsset>().WithMany().HasForeignKey(e => e.HeroImageId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(e => e.Translations).WithOne(t => t.NewsArticle).HasForeignKey(t => t.NewsArticleId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class NewsArticleTranslationConfiguration : IEntityTypeConfiguration<NewsArticleTranslation>
{
    public void Configure(EntityTypeBuilder<NewsArticleTranslation> builder)
    {
        builder.ToTable("NewsArticleTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(400).IsRequired();
        builder.Property(e => e.Summary).HasMaxLength(1000);
        builder.Property(e => e.Body).IsRequired();
        builder.Property(e => e.HeroImageCaption).HasMaxLength(500);
        builder.Property(e => e.SeoTitle).HasMaxLength(400);
        builder.Property(e => e.SeoDescription).HasMaxLength(1000);
        builder.ConfigureLanguage(e => e.Language);
    }
}
