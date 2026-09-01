using Microsoft.Extensions.Logging;
using Moq;
using SWA.Application.Common.Interfaces;
using SWA.Application.Features.Public.Search;

namespace SWA.Application.Tests.Features.Public.Search;

public class SearchQueryHandlerTests
{
    private readonly Mock<ISearchIndexer> _indexer = new();
    private readonly Mock<ILogger<SearchQueryHandler>> _logger = new();

    private SearchQueryHandler CreateHandler() => new(_indexer.Object, _logger.Object);

    [Fact]
    public async Task Handle_PassesQueryParametersThrough_AndMapsResults()
    {
        var query = new SearchQuery("water bill", "en", "Service", Page: 2, PageSize: 10);
        var updatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var hit = new SearchResultHit(Guid.NewGuid(), "Service", "water-bill", "Water Bill", "Pay your <em>water</em> bill", ["Billing"], updatedAt);

        SearchRequest? capturedRequest = null;
        _indexer
            .Setup(i => i.SearchAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SearchRequest, CancellationToken>((r, _) => capturedRequest = r)
            .ReturnsAsync(new SearchResultPage([hit], TotalCount: 1));

        var result = await CreateHandler().Handle(query, CancellationToken.None);

        Assert.NotNull(capturedRequest);
        Assert.Equal("water bill", capturedRequest!.Query);
        Assert.Equal("en", capturedRequest.Lang);
        Assert.Equal("Service", capturedRequest.ContentType);
        Assert.Equal(2, capturedRequest.Page);
        Assert.Equal(10, capturedRequest.PageSize);

        Assert.Equal(1, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        var dto = Assert.Single(result.Items);
        Assert.Equal(hit.EntityId, dto.EntityId);
        Assert.Equal(hit.Title, dto.Title);
        Assert.Equal(hit.Snippet, dto.Snippet);
        Assert.Equal(hit.TaxonomyLabels, dto.TaxonomyLabels);
    }

    [Fact]
    public async Task Handle_WhenIndexerThrows_ReturnsEmptyResultInsteadOfPropagating()
    {
        var query = new SearchQuery("anything", null, null);
        _indexer
            .Setup(i => i.SearchAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Elasticsearch is down"));

        var result = await CreateHandler().Handle(query, CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
    }
}
