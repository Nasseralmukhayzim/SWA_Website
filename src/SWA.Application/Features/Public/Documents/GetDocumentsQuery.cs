using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Application.Common.Models;
using SWA.Domain.Content.Documents;
using SWA.Domain.Enums;

namespace SWA.Application.Features.Public.Documents;

public sealed record GetDocumentsQuery(string? Lang, int? Section, Guid? CategoryId, int? Year, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<DocumentListItemDto>>, ICacheableQuery
{
    public string CacheGroup => "Documents";
    public string CacheKey => $"list:{Lang}:{Section}:{CategoryId}:{Year}:{Page}:{PageSize}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(15);
}

public sealed class GetDocumentsQueryHandler(IRepository<Document> repository, PublicContentOptions options) : IRequestHandler<GetDocumentsQuery, PagedResult<DocumentListItemDto>>
{
    public async Task<PagedResult<DocumentListItemDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Queryable()
            .PubliclyVisible(options)
            .Include(d => d.Translations)
            .Include(d => d.Category!.Translations)
            .AsQueryable();

        if (request.Section.HasValue)
        {
            var section = (DocumentSection)request.Section.Value;
            query = query.Where(d => d.Section == section);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(d => d.CategoryId == request.CategoryId);
        }

        if (request.Year.HasValue)
        {
            query = query.Where(d => d.Year == request.Year);
        }

        query = query.OrderByDescending(d => d.Year).ThenBy(d => d.SortOrder);

        var paged = await PagedResult<Document>.CreateAsync(query, request.Page, request.PageSize, cancellationToken);

        var items = paged.Items.Select(document =>
        {
            var translation = TranslationSelector.Pick(document.Translations, request.Lang);
            var categoryTranslation = document.Category is null ? null : TranslationSelector.Pick(document.Category.Translations, request.Lang);
            return new DocumentListItemDto(document.Id, document.Slug.Value, translation?.Title ?? string.Empty, (int)document.Section, document.Year, document.CoverImageId, categoryTranslation?.Name);
        }).ToList();

        return new PagedResult<DocumentListItemDto> { Items = items, Page = paged.Page, PageSize = paged.PageSize, TotalCount = paged.TotalCount };
    }
}
