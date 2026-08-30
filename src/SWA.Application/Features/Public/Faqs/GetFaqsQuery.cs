using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Application.Common.Models;
using SWA.Domain.Content.Faqs;

namespace SWA.Application.Features.Public.Faqs;

/// <summary>FAQs are shown as an accordion grouped by category, not navigated to individually — no slug-detail endpoint.</summary>
public sealed record GetFaqsQuery(string? Lang, Guid? CategoryId, int Page = 1, int PageSize = 100) : IRequest<PagedResult<FaqListItemDto>>, ICacheableQuery
{
    public string CacheGroup => "Faqs";
    public string CacheKey => $"list:{Lang}:{CategoryId}:{Page}:{PageSize}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(15);
}

public sealed class GetFaqsQueryHandler(IRepository<Faq> repository, PublicContentOptions options) : IRequestHandler<GetFaqsQuery, PagedResult<FaqListItemDto>>
{
    public async Task<PagedResult<FaqListItemDto>> Handle(GetFaqsQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Queryable()
            .PubliclyVisible(options)
            .Include(f => f.Translations)
            .Include(f => f.Category!.Translations)
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(f => f.CategoryId == request.CategoryId);
        }

        query = query.OrderBy(f => f.SortOrder);

        var paged = await PagedResult<Faq>.CreateAsync(query, request.Page, request.PageSize, cancellationToken);

        var items = paged.Items.Select(faq =>
        {
            var translation = TranslationSelector.Pick(faq.Translations, request.Lang);
            var categoryTranslation = faq.Category is null ? null : TranslationSelector.Pick(faq.Category.Translations, request.Lang);
            return new FaqListItemDto(faq.Id, faq.Slug.Value, translation?.Question ?? string.Empty, translation?.Answer ?? string.Empty, faq.CategoryId, categoryTranslation?.Name);
        }).ToList();

        return new PagedResult<FaqListItemDto> { Items = items, Page = paged.Page, PageSize = paged.PageSize, TotalCount = paged.TotalCount };
    }
}
