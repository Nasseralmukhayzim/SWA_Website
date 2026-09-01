namespace SWA.Application.Features.Public.Search;

public sealed record SearchResultDto(
    Guid EntityId,
    string ContentType,
    string Slug,
    string Title,
    string Snippet,
    IReadOnlyList<string> TaxonomyLabels,
    DateTime? UpdatedAtUtc);
