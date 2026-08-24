namespace SWA.Application.Features.Public.Pages;

public sealed record PageListItemDto(Guid Id, string Slug, string Title, string? Summary, Guid? ParentId, bool ShowInNavigation, int SortOrder);

public sealed record PageDetailDto(
    Guid Id,
    string Slug,
    string Title,
    string? Summary,
    string Body,
    string? SeoTitle,
    string? SeoDescription,
    Guid? HeroImageId,
    Guid? ParentId,
    bool ShowInNavigation,
    IReadOnlyList<PageSectionDto> Sections);

public sealed record PageSectionDto(string Kind, string? Heading, string? Intro, string? Body, IReadOnlyList<PageSectionItemDto> Items);

public sealed record PageSectionItemDto(string Title, string? Description, Guid? IconId, Guid? ImageId, string? LinkUrl);
