using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SWA.Domain.Content.Faqs;

namespace SWA.Infrastructure.Persistence.Configurations;

internal sealed class FaqConfiguration : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> builder)
    {
        builder.ToTable("Faqs");
        builder.ConfigurePublishableContent();

        builder.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(e => e.Translations).WithOne(t => t.Faq).HasForeignKey(t => t.FaqId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class FaqTranslationConfiguration : IEntityTypeConfiguration<FaqTranslation>
{
    public void Configure(EntityTypeBuilder<FaqTranslation> builder)
    {
        builder.ToTable("FaqTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Question).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.Answer).IsRequired();
        builder.ConfigureLanguage(e => e.Language);
    }
}

internal sealed class FaqCategoryConfiguration : IEntityTypeConfiguration<FaqCategory>
{
    public void Configure(EntityTypeBuilder<FaqCategory> builder)
    {
        builder.ToTable("FaqCategories");
        builder.ConfigureLookup();
        builder.HasMany(e => e.Translations).WithOne(t => t.FaqCategory).HasForeignKey(t => t.FaqCategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class FaqCategoryTranslationConfiguration : IEntityTypeConfiguration<FaqCategoryTranslation>
{
    public void Configure(EntityTypeBuilder<FaqCategoryTranslation> builder)
    {
        builder.ToTable("FaqCategoryTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.ConfigureLanguage(e => e.Language);
    }
}
