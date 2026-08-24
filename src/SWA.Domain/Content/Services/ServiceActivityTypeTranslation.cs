using SWA.Domain.Common;

namespace SWA.Domain.Content.Services;

public sealed class ServiceActivityTypeTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid ServiceActivityTypeId { get; set; }
    public ServiceActivityType ServiceActivityType { get; set; } = null!;
    public required string Name { get; set; }
    public required string Language { get; set; }
}
