using SWA.Domain.Common;

namespace SWA.Domain.Content.Faqs;

public sealed class FaqTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid FaqId { get; set; }
    public Faq Faq { get; set; } = null!;
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public required string Language { get; set; }
}
