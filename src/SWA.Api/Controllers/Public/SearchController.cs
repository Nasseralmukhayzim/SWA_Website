using MediatR;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Common.Models;
using SWA.Application.Features.Public.Search;

namespace SWA.Api.Controllers.Public;

[ApiController]
[Route("api/public/search")]
public sealed class SearchController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<SearchResultDto>> Search(
        [FromQuery] string q, [FromQuery] string? lang, [FromQuery] string? contentType,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        sender.Send(new SearchQuery(q, lang, contentType, page, pageSize), cancellationToken);
}
