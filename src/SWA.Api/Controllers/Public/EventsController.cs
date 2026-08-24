using MediatR;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Common.Models;
using SWA.Application.Features.Public.Events;

namespace SWA.Api.Controllers.Public;

[ApiController]
[Route("api/public/events")]
public sealed class EventsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<EventListItemDto>> List(
        [FromQuery] string? lang, [FromQuery] bool? upcoming, [FromQuery] Guid? eventTypeId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        sender.Send(new GetEventsQuery(lang, upcoming, eventTypeId, page, pageSize), cancellationToken);

    [HttpGet("{slug}")]
    public Task<EventDetailDto> GetBySlug(string slug, [FromQuery] string? lang, CancellationToken cancellationToken) =>
        sender.Send(new GetEventBySlugQuery(slug, lang), cancellationToken);
}
