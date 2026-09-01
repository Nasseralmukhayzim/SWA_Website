using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Exceptions;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Domain.Common;
using SWA.Domain.Content.Services;

namespace SWA.Application.Features.Public.Services;

public sealed record GetServiceBySlugQuery(string Slug, string Lang) : IRequest<ServiceDetailDto>, ICacheableQuery
{
    public string CacheGroup => "Services";
    public string CacheKey => $"slug:{Slug}:{Lang}";
    public TimeSpan CacheDuration => CacheDurations.LongLived;
}

public sealed class GetServiceBySlugQueryHandler(IRepository<Service> repository, PublicContentOptions options) : IRequestHandler<GetServiceBySlugQuery, ServiceDetailDto>
{
    public async Task<ServiceDetailDto> Handle(GetServiceBySlugQuery request, CancellationToken cancellationToken)
    {
        if (!Slug.TryCreate(request.Slug, out var slug))
        {
            throw new NotFoundException(nameof(Service), request.Slug);
        }

        var service = await repository.Queryable()
            .Include(s => s.Translations)
            .Include(s => s.Audiences).ThenInclude(a => a.Translations)
            .Include(s => s.Channels).ThenInclude(c => c.Translations)
            .Include(s => s.ServiceCategory!).ThenInclude(c => c.Translations)
            .Include(s => s.ServiceActivityType!).ThenInclude(a => a.Translations)
            .PubliclyVisible(options)
            .FirstOrDefaultAsync(s => s.Slug == slug, cancellationToken)
            ?? throw new NotFoundException(nameof(Service), request.Slug);

        var translation = TranslationSelector.Pick(service.Translations, request.Lang)
            ?? throw new NotFoundException(nameof(Service), request.Slug);

        var categoryName = service.ServiceCategory is null
            ? null
            : TranslationSelector.Pick(service.ServiceCategory.Translations, request.Lang)?.Name;
        var activityTypeName = service.ServiceActivityType is null
            ? null
            : TranslationSelector.Pick(service.ServiceActivityType.Translations, request.Lang)?.Name;

        return new ServiceDetailDto(
            service.Id,
            service.Slug.Value,
            translation.Name,
            translation.Description,
            (int)service.DeliveryType,
            service.IconId,
            service.SupportPhone,
            service.IsFeatured,
            translation.Fee,
            translation.DeliveryTime,
            translation.RequiredDocuments,
            translation.Steps,
            translation.Terms,
            translation.Objectives,
            translation.StartServiceUrl,
            translation.GuideFileId,
            service.Audiences.Select(a => a.Slug.Value).ToList(),
            service.Channels.Select(c => c.Slug.Value).ToList(),
            service.Audiences
                .Select(a => TranslationSelector.Pick(a.Translations, request.Lang)?.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name!)
                .ToList(),
            service.Channels
                .Select(c => TranslationSelector.Pick(c.Translations, request.Lang)?.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name!)
                .ToList(),
            service.ServiceCategory?.Slug.Value,
            categoryName,
            service.ServiceActivityType?.Slug.Value,
            activityTypeName,
            service.UpdatedAtUtc);
    }
}
