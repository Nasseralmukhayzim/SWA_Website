namespace SWA.Application.Features.Public.News;

public sealed record NewsListItemDto(Guid Id, string Slug, string Title, string? Summary, Guid? HeroImageId, bool IsFeatured, DateTime CreatedAtUtc, DateTime? PublishedAtUtc);

public sealed record NewsDetailDto(
    Guid Id,
    string Slug,
    string Title,
    string? Summary,
    string Body,
    string? HeroImageCaption,
    string? SeoTitle,
    string? SeoDescription,
    Guid? HeroImageId,
    bool IsFeatured,
    DateTime CreatedAtUtc,
    DateTime? PublishedAtUtc);
