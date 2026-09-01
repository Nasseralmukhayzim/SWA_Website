using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Domain.Content.Services;

namespace SWA.Infrastructure.Search.Mappers;

internal static class ServiceSearchDocumentMapper
{
    public static IEnumerable<SearchDocument> Map(Service service, string? languageFilter = null)
    {
        var languages = languageFilter is null ? service.Translations.Select(t => t.Language).Distinct() : [languageFilter];

        foreach (var lang in languages)
        {
            var translation = service.Translations.FirstOrDefault(t => t.Language == lang);
            if (translation is null)
            {
                continue;
            }

            var body = string.Join(" | ", new[]
            {
                translation.Description, translation.Fee, translation.DeliveryTime,
                translation.RequiredDocuments, translation.Steps, translation.Terms, translation.Objectives,
            }.Where(s => !string.IsNullOrWhiteSpace(s)));

            var taxonomyLabels = new List<string>();
            var taxonomySlugs = new List<string>();

            if (service.ServiceCategory is not null)
            {
                var categoryTranslation = TranslationSelector.Pick(service.ServiceCategory.Translations, lang);
                if (categoryTranslation?.Name is not null)
                {
                    taxonomyLabels.Add(categoryTranslation.Name);
                }

                taxonomySlugs.Add(service.ServiceCategory.Slug.Value);
            }

            if (service.ServiceActivityType is not null)
            {
                var activityTranslation = TranslationSelector.Pick(service.ServiceActivityType.Translations, lang);
                if (activityTranslation?.Name is not null)
                {
                    taxonomyLabels.Add(activityTranslation.Name);
                }

                taxonomySlugs.Add(service.ServiceActivityType.Slug.Value);
            }

            yield return new SearchDocument(
                Id: $"Service:{service.Id}:{lang}",
                EntityId: service.Id,
                ContentType: "Service",
                Language: lang,
                Slug: service.Slug.Value,
                Title: translation.Name,
                Body: body,
                TaxonomyLabels: taxonomyLabels,
                TaxonomySlugs: taxonomySlugs,
                UpdatedAtUtc: service.UpdatedAtUtc,
                PublishedAtUtc: service.PublishedAtUtc,
                SortOrder: service.SortOrder);
        }
    }
}
