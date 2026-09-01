using MediatR;
using Microsoft.AspNetCore.Mvc;
using SWA.Application.Features.Public.Lookups;

namespace SWA.Api.Controllers.Public;

[ApiController]
[Route("api/public/{lang:regex(^(ar|en)$)}")]
public sealed class LookupsController(ISender sender) : ControllerBase
{
    [HttpGet("event-types")]
    public Task<IReadOnlyList<LookupDto>> EventTypes([FromRoute] string lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.EventTypes, lang), cancellationToken);

    [HttpGet("faq-categories")]
    public Task<IReadOnlyList<LookupDto>> FaqCategories([FromRoute] string lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.FaqCategories, lang), cancellationToken);

    [HttpGet("service-audiences")]
    public Task<IReadOnlyList<LookupDto>> ServiceAudiences([FromRoute] string lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.ServiceAudiences, lang), cancellationToken);

    [HttpGet("service-activity-types")]
    public Task<IReadOnlyList<LookupDto>> ServiceActivityTypes([FromRoute] string lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.ServiceActivityTypes, lang), cancellationToken);

    [HttpGet("service-categories")]
    public Task<IReadOnlyList<LookupDto>> ServiceCategories([FromRoute] string lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.ServiceCategories, lang), cancellationToken);

    [HttpGet("service-channels")]
    public Task<IReadOnlyList<LookupDto>> ServiceChannels([FromRoute] string lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.ServiceChannels, lang), cancellationToken);

    [HttpGet("document-categories")]
    public Task<IReadOnlyList<LookupDto>> DocumentCategories([FromRoute] string lang, CancellationToken cancellationToken) =>
        sender.Send(new GetLookupsQuery(LookupKey.DocumentCategories, lang), cancellationToken);
}
