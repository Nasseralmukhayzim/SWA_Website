using SWA.Domain.Common;

namespace SWA.Domain.Content.Services;

public sealed class ServiceAudienceTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid ServiceAudienceId { get; set; }
    public ServiceAudience ServiceAudience { get; set; } = null!;
    public required string Name { get; set; }
    public required string Language { get; set; }
}
