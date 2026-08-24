using SWA.Domain.Common;

namespace SWA.Domain.Content.Services;

public sealed class ServiceChannelTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid ServiceChannelId { get; set; }
    public ServiceChannel ServiceChannel { get; set; } = null!;
    public required string Name { get; set; }
    public required string Language { get; set; }
}
