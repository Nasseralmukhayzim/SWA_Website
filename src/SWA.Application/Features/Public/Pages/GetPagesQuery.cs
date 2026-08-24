using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Application.Common.Models;
using SWA.Domain.Content.Pages;

namespace SWA.Application.Features.Public.Pages;

public sealed record GetPagesQuery(string? Lang, Guid? ParentId, bool? ShowInNavigation, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<PageListItemDto>>;

public sealed class GetPagesQueryHandler(IRepository<Page> repository, PublicContentOptions options) : IRequestHandler<GetPagesQuery, PagedResult<PageListItemDto>>
{
    public async Task<PagedResult<PageListItemDto>> Handle(GetPagesQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Queryable()
            .PubliclyVisible(options)
            .Include(p => p.Translations)
            .AsQueryable();

        if (request.ParentId.HasValue)
        {
            query = query.Where(p => p.ParentId == request.ParentId);
        }

        if (request.ShowInNavigation.HasValue)
        {
            query = query.Where(p => p.ShowInNavigation == request.ShowInNavigation);
        }

        query = query.OrderBy(p => p.SortOrder);

        var paged = await PagedResult<Page>.CreateAsync(query, request.Page, request.PageSize, cancellationToken);

        var items = paged.Items.Select(page =>
        {
            var translation = TranslationSelector.Pick(page.Translations, request.Lang);
            return new PageListItemDto(page.Id, page.Slug.Value, translation?.Title ?? string.Empty, translation?.Summary, page.ParentId, page.ShowInNavigation, page.SortOrder);
        }).ToList();

        return new PagedResult<PageListItemDto> { Items = items, Page = paged.Page, PageSize = paged.PageSize, TotalCount = paged.TotalCount };
    }
}
