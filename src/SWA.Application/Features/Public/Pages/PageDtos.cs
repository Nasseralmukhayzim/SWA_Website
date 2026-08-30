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
    IReadOnlyList<PageSectionDto> Sections,
    // The designs close every content page with a "last modified" line, so the date has to travel
    // with the page rather than being looked up separately.
    DateTime? UpdatedAtUtc);

public sealed record PageSectionDto(string Kind, string? Heading, string? Intro, string? Body, IReadOnlyList<PageSectionItemDto> Items);

public sealed record PageSectionItemDto(string Title, string? Description, Guid? IconId, Guid? ImageId, string? LinkUrl);
