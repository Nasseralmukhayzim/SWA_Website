using SWA.Domain.Common;
using SWA.Domain.Content.Pages;
using SWA.Infrastructure.Search.Mappers;

namespace SWA.Infrastructure.Tests.Search.Mappers;

public class PageSearchDocumentMapperTests
{
    [Fact]
    public void Map_FoldsSummaryAndBodyTogether()
    {
        var page = new Page { Id = Guid.NewGuid(), Slug = Slug.Create("about-us") };
        page.Translations.Add(new PageTranslation
        {
            Id = Guid.NewGuid(),
            PageId = page.Id,
            Title = "About Us",
            Summary = "Who we are.",
            Body = "Full page content here.",
            Language = "en",
        });

        var doc = PageSearchDocumentMapper.Map(page).Single();

        Assert.Equal("About Us", doc.Title);
        Assert.Equal("Who we are. | Full page content here.", doc.Body);
        Assert.Equal("Page", doc.ContentType);
    }

    [Fact]
    public void Map_WithNullSummary_OmitsItFromBody()
    {
        var page = new Page { Id = Guid.NewGuid(), Slug = Slug.Create("about-us") };
        page.Translations.Add(new PageTranslation { Id = Guid.NewGuid(), PageId = page.Id, Title = "About Us", Body = "Content.", Language = "en" });

        var doc = PageSearchDocumentMapper.Map(page).Single();

        Assert.Equal("Content.", doc.Body);
    }
}
