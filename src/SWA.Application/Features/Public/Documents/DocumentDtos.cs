namespace SWA.Application.Features.Public.Documents;

public sealed record DocumentListItemDto(Guid Id, string Slug, string Title, int Section, int? Year, Guid? CoverImageId, string? CategoryName);

public sealed record DocumentDetailDto(
    Guid Id,
    string Slug,
    string Title,
    string? Description,
    int Section,
    int? Year,
    Guid? CoverImageId,
    Guid? FileId,
    string? ExternalFileUrl,
    string? CategoryName);
