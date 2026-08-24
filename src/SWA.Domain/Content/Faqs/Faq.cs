namespace SWA.Domain.Content.Faqs;

public sealed class Faq : PublishableContent
{
    public Guid? CategoryId { get; set; }
    public FaqCategory? Category { get; set; }

    public List<FaqTranslation> Translations { get; set; } = [];
}
