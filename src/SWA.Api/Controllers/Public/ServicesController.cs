using MediatR;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Common.Models;
using SWA.Application.Features.Public.Services;

namespace SWA.Api.Controllers.Public;

[ApiController]
[Route("api/public/services")]
public sealed class ServicesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<ServiceListItemDto>> List(
        [FromQuery] string? lang, [FromQuery] int? deliveryType, [FromQuery] Guid? audienceId, [FromQuery] Guid? channelId, [FromQuery] Guid? categoryId, [FromQuery] Guid? activityTypeId, [FromQuery] bool? isFeatured,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        sender.Send(new GetServicesQuery(lang, deliveryType, audienceId, channelId, categoryId, activityTypeId, isFeatured, page, pageSize), cancellationToken);

    [HttpGet("{slug}")]
    public Task<ServiceDetailDto> GetBySlug(string slug, [FromQuery] string? lang, CancellationToken cancellationToken) =>
        sender.Send(new GetServiceBySlugQuery(slug, lang), cancellationToken);
}
