using SWA.Application.Common.Interfaces;
using SWA.Domain.Content.Events;

namespace SWA.Infrastructure.Search.Mappers;

internal static class EventSearchDocumentMapper
{
    public static IEnumerable<SearchDocument> Map(Event @event, string? languageFilter = null)
    {
        var languages = languageFilter is null ? @event.Translations.Select(t => t.Language).Distinct() : [languageFilter];

        foreach (var lang in languages)
        {
            var translation = @event.Translations.FirstOrDefault(t => t.Language == lang);
            if (translation is null)
            {
                continue;
            }

            var body = string.Join(" | ", new[] { translation.Description, translation.Location }.Where(s => !string.IsNullOrWhiteSpace(s)));

            yield return new SearchDocument(
                Id: $"Event:{@event.Id}:{lang}",
                EntityId: @event.Id,
                ContentType: "Event",
                Language: lang,
                Slug: @event.Slug.Value,
                Title: translation.Title,
                Body: body,
                TaxonomyLabels: [],
                TaxonomySlugs: [],
                UpdatedAtUtc: @event.UpdatedAtUtc,
                PublishedAtUtc: @event.PublishedAtUtc,
                SortOrder: @event.SortOrder);
        }
    }
}
