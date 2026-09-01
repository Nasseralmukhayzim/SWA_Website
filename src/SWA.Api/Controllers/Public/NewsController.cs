using MediatR;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Common.Models;
using SWA.Application.Features.Public.News;

namespace SWA.Api.Controllers.Public;

[ApiController]
[Route("api/public/{lang:regex(^(ar|en)$)}/news")]
public sealed class NewsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<NewsListItemDto>> List(
        [FromRoute] string lang, [FromQuery] bool? isFeatured,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        sender.Send(new GetNewsQuery(lang, isFeatured, page, pageSize), cancellationToken);

    [HttpGet("{slug}")]
    public Task<NewsDetailDto> GetBySlug(string slug, [FromRoute] string lang, CancellationToken cancellationToken) =>
        sender.Send(new GetNewsBySlugQuery(slug, lang), cancellationToken);
}
