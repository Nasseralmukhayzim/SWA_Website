using MediatR;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Features.Public.Media;

namespace SWA.Api.Controllers.Public;

[ApiController]
[Route("api/public/media")]
public sealed class MediaController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public Task<MediaAssetDto> GetById(Guid id, CancellationToken cancellationToken) =>
        sender.Send(new GetMediaByIdQuery(id), cancellationToken);
}
