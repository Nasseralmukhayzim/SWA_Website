namespace SWA.Domain.Content.Faqs;

public sealed class FaqCategory : LookupEntity
{
    public List<FaqCategoryTranslation> Translations { get; set; } = [];
}
