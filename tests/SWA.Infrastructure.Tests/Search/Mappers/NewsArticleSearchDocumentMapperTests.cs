using SWA.Domain.Common;
using SWA.Domain.Content.News;
using SWA.Infrastructure.Search.Mappers;

namespace SWA.Infrastructure.Tests.Search.Mappers;

public class NewsArticleSearchDocumentMapperTests
{
    [Fact]
    public void Map_FoldsSummaryAndBodyTogether()
    {
        var article = new NewsArticle { Id = Guid.NewGuid(), Slug = Slug.Create("new-reservoir-opens") };
        article.Translations.Add(new NewsArticleTranslation
        {
            Id = Guid.NewGuid(),
            NewsArticleId = article.Id,
            Title = "New Reservoir Opens",
            Summary = "A short summary.",
            Body = "The full article body.",
            Language = "en",
        });

        var doc = NewsArticleSearchDocumentMapper.Map(article).Single();

        Assert.Equal("New Reservoir Opens", doc.Title);
        Assert.Equal("A short summary. | The full article body.", doc.Body);
        Assert.Equal("NewsArticle", doc.ContentType);
    }
}
