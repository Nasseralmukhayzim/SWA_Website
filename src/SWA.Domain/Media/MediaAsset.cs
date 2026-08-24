using SWA.Domain.Common;
using SWA.Domain.Enums;

namespace SWA.Domain.Media;

public sealed class MediaAsset : AuditableEntity
{
    public required string StorageKey { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public long SizeInBytes { get; set; }
    public MediaKind Kind { get; set; }
    public string? TitleAr { get; set; }
    public string? TitleEn { get; set; }
    public string? AltTextAr { get; set; }
    public string? AltTextEn { get; set; }
}
