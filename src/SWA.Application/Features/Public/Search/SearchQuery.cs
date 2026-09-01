using MediatR;
using Microsoft.Extensions.Logging;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Models;

namespace SWA.Application.Features.Public.Search;

public sealed record SearchQuery(string Q, string? Lang, string? ContentType, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<SearchResultDto>>, ICacheableQuery
{
    public string CacheGroup => "Search";
    public string CacheKey => $"{Q}:{Lang}:{ContentType}:{Page}:{PageSize}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(3);
}

public sealed class SearchQueryHandler(ISearchIndexer indexer, ILogger<SearchQueryHandler> logger)
    : IRequestHandler<SearchQuery, PagedResult<SearchResultDto>>
{
    public async Task<PagedResult<SearchResultDto>> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        SearchResultPage page;
        try
        {
            page = await indexer.SearchAsync(
                new SearchRequest(request.Q, request.Lang, request.ContentType, request.Page, request.PageSize),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Elasticsearch unavailable while searching for {Query}; returning an empty result.", request.Q);
            return new PagedResult<SearchResultDto> { Items = [], Page = request.Page, PageSize = request.PageSize, TotalCount = 0 };
        }

        var items = page.Items
            .Select(hit => new SearchResultDto(hit.EntityId, hit.ContentType, hit.Slug, hit.Title, hit.Snippet, hit.TaxonomyLabels, hit.UpdatedAtUtc))
            .ToList();

        return new PagedResult<SearchResultDto> { Items = items, Page = request.Page, PageSize = request.PageSize, TotalCount = page.TotalCount };
    }
}
