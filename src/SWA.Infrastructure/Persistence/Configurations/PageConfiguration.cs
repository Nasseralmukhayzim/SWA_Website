using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SWA.Domain.Content.Pages;
using SWA.Domain.Media;

namespace SWA.Infrastructure.Persistence.Configurations;

internal sealed class PageConfiguration : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("Pages");
        builder.ConfigurePublishableContent();
        builder.Property(e => e.ShowInNavigation).IsRequired();
        builder.Property(e => e.ViewCount).IsRequired();

        builder.HasOne(e => e.Parent).WithMany().HasForeignKey(e => e.ParentId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<MediaAsset>().WithMany().HasForeignKey(e => e.HeroImageId).OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(e => e.Translations).WithOne(t => t.Page).HasForeignKey(t => t.PageId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class PageTranslationConfiguration : IEntityTypeConfiguration<PageTranslation>
{
    public void Configure(EntityTypeBuilder<PageTranslation> builder)
    {
        builder.ToTable("PageTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(400).IsRequired();
        builder.Property(e => e.Summary).HasMaxLength(1000);
        builder.Property(e => e.Body).IsRequired();
        builder.Property(e => e.SeoTitle).HasMaxLength(400);
        builder.Property(e => e.SeoDescription).HasMaxLength(1000);
        builder.ConfigureLanguage(e => e.Language);

        builder.Property(e => e.Sections)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                sections => sections == null ? null : JsonSerializer.Serialize(sections, JsonOptions),
                json => string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<List<PageSection>>(json, JsonOptions),
                new ValueComparer<List<PageSection>?>(
                    (a, b) => JsonSerializer.Serialize(a, JsonOptions) == JsonSerializer.Serialize(b, JsonOptions),
                    v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOptions).GetHashCode(),
                    v => v == null ? null : JsonSerializer.Deserialize<List<PageSection>>(JsonSerializer.Serialize(v, JsonOptions), JsonOptions)));
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
    };
}
