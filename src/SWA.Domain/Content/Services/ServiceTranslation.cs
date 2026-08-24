using SWA.Domain.Common;

namespace SWA.Domain.Content.Services;

public sealed class ServiceTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Fee { get; set; }
    public string? DeliveryTime { get; set; }
    public string? RequiredDocuments { get; set; }
    public string? Steps { get; set; }
    public string? Terms { get; set; }
    public string? Objectives { get; set; }
    public string? StartServiceUrl { get; set; }
    public Guid? GuideFileId { get; set; }
    public required string Language { get; set; }
}
