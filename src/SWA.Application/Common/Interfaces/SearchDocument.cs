namespace SWA.Application.Common.Interfaces;

/// <summary>
/// One indexable unit — a single translation of a publishable content entity. Search indexes
/// one document per translation (not per entity) so relevance scoring and highlighting stay
/// scoped to a single language instead of mixing English and Arabic text in one document.
/// </summary>
public sealed record SearchDocument(
    string Id,
    Guid EntityId,
    string ContentType,
    string Language,
    string Slug,
    string Title,
    string Body,
    IReadOnlyList<string> TaxonomyLabels,
    IReadOnlyList<string> TaxonomySlugs,
    DateTime? UpdatedAtUtc,
    DateTime? PublishedAtUtc,
    int SortOrder,
    string? AttachmentBase64 = null);
