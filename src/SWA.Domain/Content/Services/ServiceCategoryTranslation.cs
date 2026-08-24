using SWA.Domain.Common;

namespace SWA.Domain.Content.Services;

public sealed class ServiceCategoryTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid ServiceCategoryId { get; set; }
    public ServiceCategory ServiceCategory { get; set; } = null!;
    public required string Name { get; set; }
    public required string Language { get; set; }
}
