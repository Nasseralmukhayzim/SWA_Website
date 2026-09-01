using SWA.Application.Common.Interfaces;
using SWA.Domain.Content.News;

namespace SWA.Infrastructure.Search.Mappers;

internal static class NewsArticleSearchDocumentMapper
{
    public static IEnumerable<SearchDocument> Map(NewsArticle article, string? languageFilter = null)
    {
        var languages = languageFilter is null ? article.Translations.Select(t => t.Language).Distinct() : [languageFilter];

        foreach (var lang in languages)
        {
            var translation = article.Translations.FirstOrDefault(t => t.Language == lang);
            if (translation is null)
            {
                continue;
            }

            var body = string.Join(" | ", new[] { translation.Summary, translation.Body }.Where(s => !string.IsNullOrWhiteSpace(s)));

            yield return new SearchDocument(
                Id: $"NewsArticle:{article.Id}:{lang}",
                EntityId: article.Id,
                ContentType: "NewsArticle",
                Language: lang,
                Slug: article.Slug.Value,
                Title: translation.Title,
                Body: body,
                TaxonomyLabels: [],
                TaxonomySlugs: [],
                UpdatedAtUtc: article.UpdatedAtUtc,
                PublishedAtUtc: article.PublishedAtUtc,
                SortOrder: article.SortOrder);
        }
    }
}
