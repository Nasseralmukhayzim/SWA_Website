using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Domain.Content.Documents;

namespace SWA.Infrastructure.Search.Mappers;

internal static class DocumentSearchDocumentMapper
{
    public static IEnumerable<SearchDocument> Map(
        Document document, string? languageFilter = null, IReadOnlyDictionary<Guid, string>? attachmentsByFileId = null)
    {
        var languages = languageFilter is null ? document.Translations.Select(t => t.Language).Distinct() : [languageFilter];

        foreach (var lang in languages)
        {
            var translation = document.Translations.FirstOrDefault(t => t.Language == lang);
            if (translation is null)
            {
                continue;
            }

            var taxonomyLabels = new List<string>();
            var taxonomySlugs = new List<string>();
            if (document.Category is not null)
            {
                var categoryTranslation = TranslationSelector.Pick(document.Category.Translations, lang);
                if (categoryTranslation?.Name is not null)
                {
                    taxonomyLabels.Add(categoryTranslation.Name);
                }

                taxonomySlugs.Add(document.Category.Slug.Value);
            }

            var attachmentBase64 = translation.FileId is Guid fileId && attachmentsByFileId is not null && attachmentsByFileId.TryGetValue(fileId, out var base64)
                ? base64
                : null;

            yield return new SearchDocument(
                Id: $"Document:{document.Id}:{lang}",
                EntityId: document.Id,
                ContentType: "Document",
                Language: lang,
                Slug: document.Slug.Value,
                Title: translation.Title,
                Body: translation.Description ?? string.Empty,
                TaxonomyLabels: taxonomyLabels,
                TaxonomySlugs: taxonomySlugs,
                UpdatedAtUtc: document.UpdatedAtUtc,
                PublishedAtUtc: document.PublishedAtUtc,
                SortOrder: document.SortOrder,
                AttachmentBase64: attachmentBase64);
        }
    }
}
