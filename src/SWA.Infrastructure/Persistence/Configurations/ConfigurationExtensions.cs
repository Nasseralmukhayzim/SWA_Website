using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SWA.Domain.Common;
using SWA.Domain.Content;

namespace SWA.Infrastructure.Persistence.Configurations;

internal static class ConfigurationExtensions
{
    public static void ConfigureAuditable<T>(this EntityTypeBuilder<T> builder) where T : AuditableEntity
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CreatedAtUtc).IsRequired();
        builder.Property(e => e.UpdatedAtUtc);
        builder.Property(e => e.IsDeleted).IsRequired();
    }

    public static void ConfigureSlug<T>(this EntityTypeBuilder<T> builder, System.Linq.Expressions.Expression<Func<T, Slug>> slugSelector) where T : class
    {
        builder.Property(slugSelector)
            .HasConversion(slug => slug.Value, value => Slug.Create(value))
            .HasColumnName("Slug")
            .HasColumnType("varchar(200)")
            .IsRequired();
    }

    public static void ConfigureLookup<T>(this EntityTypeBuilder<T> builder) where T : LookupEntity
    {
        builder.ConfigureAuditable();
        builder.ConfigureSlug(e => e.Slug);
        builder.Property(e => e.SortOrder).IsRequired();
        builder.Property(e => e.IsActive).IsRequired();
    }

    public static void ConfigurePublishableContent<T>(this EntityTypeBuilder<T> builder) where T : PublishableContent
    {
        builder.ConfigureAuditable();
        builder.ConfigureSlug(e => e.Slug);
        builder.Property(e => e.SortOrder).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.DeletionStatus).IsRequired().HasDefaultValue(Domain.Enums.DeletionRequestStatus.None);
    }

    public static void ConfigureLanguage<T>(this EntityTypeBuilder<T> builder, System.Linq.Expressions.Expression<Func<T, string>> languageSelector) where T : class
    {
        builder.Property(languageSelector).HasColumnType("varchar(8)").IsRequired();
    }
}
