using SWA.Application.Common.Interfaces;
using SWA.Domain.Content.Pages;

namespace SWA.Infrastructure.Search.Mappers;

internal static class PageSearchDocumentMapper
{
    public static IEnumerable<SearchDocument> Map(Page page, string? languageFilter = null)
    {
        var languages = languageFilter is null ? page.Translations.Select(t => t.Language).Distinct() : [languageFilter];

        foreach (var lang in languages)
        {
            var translation = page.Translations.FirstOrDefault(t => t.Language == lang);
            if (translation is null)
            {
                continue;
            }

            var body = string.Join(" | ", new[] { translation.Summary, translation.Body }.Where(s => !string.IsNullOrWhiteSpace(s)));

            yield return new SearchDocument(
                Id: $"Page:{page.Id}:{lang}",
                EntityId: page.Id,
                ContentType: "Page",
                Language: lang,
                Slug: page.Slug.Value,
                Title: translation.Title,
                Body: body,
                TaxonomyLabels: [],
                TaxonomySlugs: [],
                UpdatedAtUtc: page.UpdatedAtUtc,
                PublishedAtUtc: page.PublishedAtUtc,
                SortOrder: page.SortOrder);
        }
    }
}
