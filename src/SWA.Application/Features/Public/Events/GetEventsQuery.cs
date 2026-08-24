using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Application.Common.Models;
using SWA.Domain.Content.Events;

namespace SWA.Application.Features.Public.Events;

public sealed record GetEventsQuery(string? Lang, bool? Upcoming, Guid? EventTypeId, int Page = 1, int PageSize = 20) : IRequest<PagedResult<EventListItemDto>>;

public sealed class GetEventsQueryHandler(IRepository<Event> repository, PublicContentOptions options) : IRequestHandler<GetEventsQuery, PagedResult<EventListItemDto>>
{
    public async Task<PagedResult<EventListItemDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Queryable()
            .PubliclyVisible(options)
            .Include(e => e.Translations)
            .Include(e => e.EventType!.Translations)
            .AsQueryable();

        if (request.Upcoming == true)
        {
            var now = DateTime.UtcNow;
            query = query.Where(e => e.StartsAtUtc >= now);
        }

        if (request.EventTypeId.HasValue)
        {
            query = query.Where(e => e.EventTypeId == request.EventTypeId);
        }

        query = query.OrderBy(e => e.StartsAtUtc);

        var paged = await PagedResult<Event>.CreateAsync(query, request.Page, request.PageSize, cancellationToken);

        var items = paged.Items.Select(ev =>
        {
            var translation = TranslationSelector.Pick(ev.Translations, request.Lang);
            var typeTranslation = ev.EventType is null ? null : TranslationSelector.Pick(ev.EventType.Translations, request.Lang);
            return new EventListItemDto(ev.Id, ev.Slug.Value, translation?.Title ?? string.Empty, translation?.Location, ev.StartsAtUtc, ev.EndsAtUtc, ev.ImageId, typeTranslation?.Name);
        }).ToList();

        return new PagedResult<EventListItemDto> { Items = items, Page = paged.Page, PageSize = paged.PageSize, TotalCount = paged.TotalCount };
    }
}
