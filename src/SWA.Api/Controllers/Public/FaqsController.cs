using MediatR;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Common.Models;
using SWA.Application.Features.Public.Faqs;

namespace SWA.Api.Controllers.Public;

[ApiController]
[Route("api/public/faqs")]
public sealed class FaqsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<FaqListItemDto>> List(
        [FromQuery] string? lang, [FromQuery] Guid? categoryId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 100, CancellationToken cancellationToken = default) =>
        sender.Send(new GetFaqsQuery(lang, categoryId, page, pageSize), cancellationToken);
}
