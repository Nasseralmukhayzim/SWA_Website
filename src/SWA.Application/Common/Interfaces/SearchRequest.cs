namespace SWA.Application.Common.Interfaces;

public sealed record SearchRequest(string Query, string? Lang, string? ContentType, int Page, int PageSize);

public sealed record SearchResultHit(
    Guid EntityId,
    string ContentType,
    string Slug,
    string Title,
    string Snippet,
    IReadOnlyList<string> TaxonomyLabels,
    DateTime? UpdatedAtUtc);

public sealed record SearchResultPage(IReadOnlyList<SearchResultHit> Items, int TotalCount);
