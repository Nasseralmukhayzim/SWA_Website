using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Exceptions;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Domain.Common;
using SWA.Domain.Content.Events;

namespace SWA.Application.Features.Public.Events;

public sealed record GetEventBySlugQuery(string Slug, string Lang) : IRequest<EventDetailDto>, ICacheableQuery
{
    public string CacheGroup => "Events";
    public string CacheKey => $"slug:{Slug}:{Lang}";
    public TimeSpan CacheDuration => CacheDurations.LongLived;
}

public sealed class GetEventBySlugQueryHandler(IRepository<Event> repository, PublicContentOptions options) : IRequestHandler<GetEventBySlugQuery, EventDetailDto>
{
    public async Task<EventDetailDto> Handle(GetEventBySlugQuery request, CancellationToken cancellationToken)
    {
        if (!Slug.TryCreate(request.Slug, out var slug))
        {
            throw new NotFoundException(nameof(Event), request.Slug);
        }

        var ev = await repository.Queryable()
            .Include(e => e.Translations)
            .Include(e => e.EventType!.Translations)
            .PubliclyVisible(options)
            .FirstOrDefaultAsync(e => e.Slug == slug, cancellationToken)
            ?? throw new NotFoundException(nameof(Event), request.Slug);

        var translation = TranslationSelector.Pick(ev.Translations, request.Lang)
            ?? throw new NotFoundException(nameof(Event), request.Slug);
        var typeTranslation = ev.EventType is null ? null : TranslationSelector.Pick(ev.EventType.Translations, request.Lang);

        return new EventDetailDto(
            ev.Id,
            ev.Slug.Value,
            translation.Title,
            translation.Description,
            translation.Location,
            ev.StartsAtUtc,
            ev.EndsAtUtc,
            ev.ImageId,
            ev.RegistrationUrl,
            typeTranslation?.Name);
    }
}
