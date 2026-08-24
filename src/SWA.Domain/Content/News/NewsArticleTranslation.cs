using SWA.Domain.Common;

namespace SWA.Domain.Content.News;

public sealed class NewsArticleTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid NewsArticleId { get; set; }
    public NewsArticle NewsArticle { get; set; } = null!;
    public required string Title { get; set; }
    public string? Summary { get; set; }
    public required string Body { get; set; }
    public string? HeroImageCaption { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public required string Language { get; set; }
}
