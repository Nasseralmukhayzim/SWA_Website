using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Domain.Content.Faqs;

namespace SWA.Infrastructure.Search.Mappers;

internal static class FaqSearchDocumentMapper
{
    public static IEnumerable<SearchDocument> Map(Faq faq, string? languageFilter = null)
    {
        var languages = languageFilter is null ? faq.Translations.Select(t => t.Language).Distinct() : [languageFilter];

        foreach (var lang in languages)
        {
            var translation = faq.Translations.FirstOrDefault(t => t.Language == lang);
            if (translation is null)
            {
                continue;
            }

            var taxonomyLabels = new List<string>();
            var taxonomySlugs = new List<string>();
            if (faq.Category is not null)
            {
                var categoryTranslation = TranslationSelector.Pick(faq.Category.Translations, lang);
                if (categoryTranslation?.Name is not null)
                {
                    taxonomyLabels.Add(categoryTranslation.Name);
                }

                taxonomySlugs.Add(faq.Category.Slug.Value);
            }

            yield return new SearchDocument(
                Id: $"Faq:{faq.Id}:{lang}",
                EntityId: faq.Id,
                ContentType: "Faq",
                Language: lang,
                Slug: faq.Slug.Value,
                Title: translation.Question,
                Body: translation.Answer,
                TaxonomyLabels: taxonomyLabels,
                TaxonomySlugs: taxonomySlugs,
                UpdatedAtUtc: faq.UpdatedAtUtc,
                PublishedAtUtc: faq.PublishedAtUtc,
                SortOrder: faq.SortOrder);
        }
    }
}
