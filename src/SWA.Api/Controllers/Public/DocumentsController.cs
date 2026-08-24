using MediatR;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Common.Models;
using SWA.Application.Features.Public.Documents;

namespace SWA.Api.Controllers.Public;

[ApiController]
[Route("api/public/documents")]
public sealed class DocumentsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<DocumentListItemDto>> List(
        [FromQuery] string? lang, [FromQuery] int? section, [FromQuery] Guid? categoryId, [FromQuery] int? year,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        sender.Send(new GetDocumentsQuery(lang, section, categoryId, year, page, pageSize), cancellationToken);

    [HttpGet("{slug}")]
    public Task<DocumentDetailDto> GetBySlug(string slug, [FromQuery] string? lang, CancellationToken cancellationToken) =>
        sender.Send(new GetDocumentBySlugQuery(slug, lang), cancellationToken);
}
