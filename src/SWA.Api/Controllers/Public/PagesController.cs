using MediatR;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Common.Models;
using SWA.Application.Features.Public.Pages;

namespace SWA.Api.Controllers.Public;

[ApiController]
[Route("api/public/{lang:regex(^(ar|en)$)}/pages")]
public sealed class PagesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<PageListItemDto>> List(
        [FromRoute] string lang, [FromQuery] Guid? parentId, [FromQuery] bool? showInNavigation,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        sender.Send(new GetPagesQuery(lang, parentId, showInNavigation, page, pageSize), cancellationToken);

    [HttpGet("{slug}")]
    public Task<PageDetailDto> GetBySlug(string slug, [FromRoute] string lang, CancellationToken cancellationToken) =>
        sender.Send(new GetPageBySlugQuery(slug, lang), cancellationToken);
}
