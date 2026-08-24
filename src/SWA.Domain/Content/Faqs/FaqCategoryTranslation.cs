using SWA.Domain.Common;

namespace SWA.Domain.Content.Faqs;

public sealed class FaqCategoryTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid FaqCategoryId { get; set; }
    public FaqCategory FaqCategory { get; set; } = null!;
    public required string Name { get; set; }
    public required string Language { get; set; }
}
