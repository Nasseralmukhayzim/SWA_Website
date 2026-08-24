using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SWA.Domain.Content.Services;
using SWA.Domain.Media;

namespace SWA.Infrastructure.Persistence.Configurations;

internal sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");
        builder.ConfigurePublishableContent();
        builder.Property(e => e.DeliveryType).IsRequired();
        builder.Property(e => e.SupportPhone).HasColumnType("varchar(32)");
        builder.Property(e => e.IsFeatured).IsRequired();

        builder.HasOne(e => e.FaqCategory).WithMany().HasForeignKey(e => e.FaqCategoryId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(e => e.ServiceCategory).WithMany(c => c.Services).HasForeignKey(e => e.ServiceCategoryId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(e => e.ServiceActivityType).WithMany(a => a.Services).HasForeignKey(e => e.ServiceActivityTypeId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<MediaAsset>().WithMany().HasForeignKey(e => e.IconId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(e => e.Translations).WithOne(t => t.Service).HasForeignKey(t => t.ServiceId).OnDelete(DeleteBehavior.Cascade);

        // Explicit join-column names ("ServiceId", not EF's default pluralized "ServicesId") to match the exported schema.
        builder.HasMany(e => e.Audiences).WithMany(a => a.Services)
            .UsingEntity<Dictionary<string, object>>(
                "ServiceAudienceLinks",
                j => j.HasOne<ServiceAudience>().WithMany().HasForeignKey("AudiencesId"),
                j => j.HasOne<Service>().WithMany().HasForeignKey("ServiceId"));

        builder.HasMany(e => e.Channels).WithMany(c => c.Services)
            .UsingEntity<Dictionary<string, object>>(
                "ServiceChannelLinks",
                j => j.HasOne<ServiceChannel>().WithMany().HasForeignKey("ChannelsId"),
                j => j.HasOne<Service>().WithMany().HasForeignKey("ServiceId"));
    }
}

internal sealed class ServiceTranslationConfiguration : IEntityTypeConfiguration<ServiceTranslation>
{
    public void Configure(EntityTypeBuilder<ServiceTranslation> builder)
    {
        builder.ToTable("ServiceTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.Property(e => e.Fee).HasMaxLength(300);
        builder.Property(e => e.DeliveryTime).HasMaxLength(300);
        builder.Property(e => e.StartServiceUrl).HasMaxLength(2048);
        builder.ConfigureLanguage(e => e.Language);
    }
}

internal sealed class ServiceAudienceConfiguration : IEntityTypeConfiguration<ServiceAudience>
{
    public void Configure(EntityTypeBuilder<ServiceAudience> builder)
    {
        builder.ToTable("ServiceAudiences");
        builder.ConfigureLookup();
        builder.HasMany(e => e.Translations).WithOne(t => t.ServiceAudience).HasForeignKey(t => t.ServiceAudienceId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ServiceAudienceTranslationConfiguration : IEntityTypeConfiguration<ServiceAudienceTranslation>
{
    public void Configure(EntityTypeBuilder<ServiceAudienceTranslation> builder)
    {
        builder.ToTable("ServiceAudienceTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.ConfigureLanguage(e => e.Language);
    }
}

internal sealed class ServiceChannelConfiguration : IEntityTypeConfiguration<ServiceChannel>
{
    public void Configure(EntityTypeBuilder<ServiceChannel> builder)
    {
        builder.ToTable("ServiceChannels");
        builder.ConfigureLookup();
        builder.HasMany(e => e.Translations).WithOne(t => t.ServiceChannel).HasForeignKey(t => t.ServiceChannelId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ServiceChannelTranslationConfiguration : IEntityTypeConfiguration<ServiceChannelTranslation>
{
    public void Configure(EntityTypeBuilder<ServiceChannelTranslation> builder)
    {
        builder.ToTable("ServiceChannelTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.ConfigureLanguage(e => e.Language);
    }
}

internal sealed class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.ToTable("ServiceCategories");
        builder.ConfigureLookup();
        builder.HasMany(e => e.Translations).WithOne(t => t.ServiceCategory).HasForeignKey(t => t.ServiceCategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ServiceCategoryTranslationConfiguration : IEntityTypeConfiguration<ServiceCategoryTranslation>
{
    public void Configure(EntityTypeBuilder<ServiceCategoryTranslation> builder)
    {
        builder.ToTable("ServiceCategoryTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.ConfigureLanguage(e => e.Language);
    }
}

internal sealed class ServiceActivityTypeConfiguration : IEntityTypeConfiguration<ServiceActivityType>
{
    public void Configure(EntityTypeBuilder<ServiceActivityType> builder)
    {
        builder.ToTable("ServiceActivityTypes");
        builder.ConfigureLookup();
        builder.HasMany(e => e.Translations).WithOne(t => t.ServiceActivityType).HasForeignKey(t => t.ServiceActivityTypeId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ServiceActivityTypeTranslationConfiguration : IEntityTypeConfiguration<ServiceActivityTypeTranslation>
{
    public void Configure(EntityTypeBuilder<ServiceActivityTypeTranslation> builder)
    {
        builder.ToTable("ServiceActivityTypeTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.ConfigureLanguage(e => e.Language);
    }
}
