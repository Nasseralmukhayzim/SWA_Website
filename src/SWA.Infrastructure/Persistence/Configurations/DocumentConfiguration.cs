using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SWA.Domain.Content.Documents;
using SWA.Domain.Media;

namespace SWA.Infrastructure.Persistence.Configurations;

internal sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");
        builder.ConfigurePublishableContent();
        builder.Property(e => e.Section).IsRequired();

        builder.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<MediaAsset>().WithMany().HasForeignKey(e => e.CoverImageId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(e => e.Translations).WithOne(t => t.Document).HasForeignKey(t => t.DocumentId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class DocumentTranslationConfiguration : IEntityTypeConfiguration<DocumentTranslation>
{
    public void Configure(EntityTypeBuilder<DocumentTranslation> builder)
    {
        builder.ToTable("DocumentTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(400).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.ExternalFileUrl).HasMaxLength(2048);
        builder.ConfigureLanguage(e => e.Language);

        builder.HasOne<MediaAsset>().WithMany().HasForeignKey(e => e.FileId).OnDelete(DeleteBehavior.NoAction);
    }
}

internal sealed class DocumentCategoryConfiguration : IEntityTypeConfiguration<DocumentCategory>
{
    public void Configure(EntityTypeBuilder<DocumentCategory> builder)
    {
        builder.ToTable("DocumentCategories");
        builder.ConfigureLookup();
        builder.Property(e => e.Section).IsRequired();
        builder.HasMany(e => e.Translations).WithOne(t => t.DocumentCategory).HasForeignKey(t => t.DocumentCategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class DocumentCategoryTranslationConfiguration : IEntityTypeConfiguration<DocumentCategoryTranslation>
{
    public void Configure(EntityTypeBuilder<DocumentCategoryTranslation> builder)
    {
        builder.ToTable("DocumentCategoryTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.ConfigureLanguage(e => e.Language);
    }
}
