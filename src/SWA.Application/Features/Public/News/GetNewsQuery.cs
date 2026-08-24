using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Application.Common.Models;
using SWA.Domain.Content.News;

namespace SWA.Application.Features.Public.News;

public sealed record GetNewsQuery(string? Lang, bool? IsFeatured, int Page = 1, int PageSize = 20) : IRequest<PagedResult<NewsListItemDto>>;

public sealed class GetNewsQueryHandler(IRepository<NewsArticle> repository, PublicContentOptions options) : IRequestHandler<GetNewsQuery, PagedResult<NewsListItemDto>>
{
    public async Task<PagedResult<NewsListItemDto>> Handle(GetNewsQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Queryable()
            .PubliclyVisible(options)
            .Include(n => n.Translations)
            .AsQueryable();

        if (request.IsFeatured.HasValue)
        {
            query = query.Where(n => n.IsFeatured == request.IsFeatured);
        }

        query = query.OrderByDescending(n => n.PublishedAtUtc).ThenBy(n => n.SortOrder);

        var paged = await PagedResult<NewsArticle>.CreateAsync(query, request.Page, request.PageSize, cancellationToken);

        var items = paged.Items.Select(article =>
        {
            var translation = TranslationSelector.Pick(article.Translations, request.Lang);
            return new NewsListItemDto(article.Id, article.Slug.Value, translation?.Title ?? string.Empty, translation?.Summary, article.HeroImageId, article.IsFeatured, article.CreatedAtUtc, article.PublishedAtUtc);
        }).ToList();

        return new PagedResult<NewsListItemDto> { Items = items, Page = paged.Page, PageSize = paged.PageSize, TotalCount = paged.TotalCount };
    }
}
