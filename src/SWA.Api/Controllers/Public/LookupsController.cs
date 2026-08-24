using MediatR;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Features.Public.Lookups;

namespace SWA.Api.Controllers.Public;

[ApiController]
[Route("api/public/lookups")]
public sealed class LookupsController(ISender sender) : ControllerBase
{
    [HttpGet("event-types")]
    public Task<IReadOnlyList<LookupDto>> EventTypes([FromQuery] string? lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.EventTypes, lang), cancellationToken);

    [HttpGet("faq-categories")]
    public Task<IReadOnlyList<LookupDto>> FaqCategories([FromQuery] string? lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.FaqCategories, lang), cancellationToken);

    [HttpGet("service-audiences")]
    public Task<IReadOnlyList<LookupDto>> ServiceAudiences([FromQuery] string? lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.ServiceAudiences, lang), cancellationToken);

    [HttpGet("service-activity-types")]
    public Task<IReadOnlyList<LookupDto>> ServiceActivityTypes([FromQuery] string? lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.ServiceActivityTypes, lang), cancellationToken);

    [HttpGet("service-categories")]
    public Task<IReadOnlyList<LookupDto>> ServiceCategories([FromQuery] string? lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.ServiceCategories, lang), cancellationToken);

    [HttpGet("service-channels")]
    public Task<IReadOnlyList<LookupDto>> ServiceChannels([FromQuery] string? lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.ServiceChannels, lang), cancellationToken);

    [HttpGet("document-categories")]
    public Task<IReadOnlyList<LookupDto>> DocumentCategories([FromQuery] string? lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.DocumentCategories, lang), cancellationToken);
}
