namespace SWA.Domain.Content.News;

public sealed class NewsArticle : PublishableContent
{
    public Guid? HeroImageId { get; set; }
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }

    public List<NewsArticleTranslation> Translations { get; set; } = [];
}
