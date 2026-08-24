using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SWA.Domain.Content.Events;
using SWA.Domain.Media;

namespace SWA.Infrastructure.Persistence.Configurations;

internal sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");
        builder.ConfigurePublishableContent();
        builder.Property(e => e.StartsAtUtc).IsRequired();
        builder.Property(e => e.RegistrationUrl).HasMaxLength(2048);

        builder.HasOne(e => e.EventType).WithMany().HasForeignKey(e => e.EventTypeId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<MediaAsset>().WithMany().HasForeignKey(e => e.ImageId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(e => e.Translations).WithOne(t => t.Event).HasForeignKey(t => t.EventId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class EventTranslationConfiguration : IEntityTypeConfiguration<EventTranslation>
{
    public void Configure(EntityTypeBuilder<EventTranslation> builder)
    {
        builder.ToTable("EventTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(400).IsRequired();
        builder.Property(e => e.Description);
        builder.Property(e => e.Location).HasMaxLength(300);
        builder.ConfigureLanguage(e => e.Language);
    }
}

internal sealed class EventTypeConfiguration : IEntityTypeConfiguration<EventType>
{
    public void Configure(EntityTypeBuilder<EventType> builder)
    {
        builder.ToTable("EventTypes");
        builder.ConfigureLookup();
        builder.HasMany(e => e.Translations).WithOne(t => t.EventType).HasForeignKey(t => t.EventTypeId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class EventTypeTranslationConfiguration : IEntityTypeConfiguration<EventTypeTranslation>
{
    public void Configure(EntityTypeBuilder<EventTypeTranslation> builder)
    {
        builder.ToTable("EventTypeTranslations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(300).IsRequired();
        builder.ConfigureLanguage(e => e.Language);
    }
}
