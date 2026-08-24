using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SWA.Domain.Media;

namespace SWA.Infrastructure.Persistence.Configurations;

internal sealed class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("MediaAssets");
        builder.ConfigureAuditable();
        builder.Property(e => e.StorageKey).HasMaxLength(512).IsRequired();
        builder.Property(e => e.OriginalFileName).HasMaxLength(260).IsRequired();
        builder.Property(e => e.ContentType).HasColumnType("varchar(200)").IsRequired();
        builder.Property(e => e.SizeInBytes).IsRequired();
        builder.Property(e => e.Kind).IsRequired();
        builder.Property(e => e.TitleAr).HasMaxLength(300);
        builder.Property(e => e.TitleEn).HasMaxLength(300);
        builder.Property(e => e.AltTextAr).HasMaxLength(500);
        builder.Property(e => e.AltTextEn).HasMaxLength(500);
    }
}
