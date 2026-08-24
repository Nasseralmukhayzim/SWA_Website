using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Application.Common.Models;
using SWA.Domain.Content.Services;
using SWA.Domain.Enums;

namespace SWA.Application.Features.Public.Services;

public sealed record GetServicesQuery(string? Lang, int? DeliveryType, Guid? AudienceId, Guid? ChannelId, Guid? CategoryId, Guid? ActivityTypeId, bool? IsFeatured, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<ServiceListItemDto>>;

public sealed class GetServicesQueryHandler(IRepository<Service> repository, PublicContentOptions options) : IRequestHandler<GetServicesQuery, PagedResult<ServiceListItemDto>>
{
    public async Task<PagedResult<ServiceListItemDto>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
    {
        var query = repository.Queryable()
            .PubliclyVisible(options)
            .Include(s => s.Translations)
            .Include(s => s.Audiences).ThenInclude(a => a.Translations)
            .Include(s => s.ServiceCategory!).ThenInclude(c => c.Translations)
            .Include(s => s.ServiceActivityType!).ThenInclude(a => a.Translations)
            .AsQueryable();

        if (request.DeliveryType.HasValue)
        {
            var deliveryType = (ServiceDeliveryType)request.DeliveryType.Value;
            query = query.Where(s => s.DeliveryType == deliveryType);
        }

        if (request.AudienceId.HasValue)
        {
            query = query.Where(s => s.Audiences.Any(a => a.Id == request.AudienceId));
        }

        if (request.ChannelId.HasValue)
        {
            query = query.Where(s => s.Channels.Any(c => c.Id == request.ChannelId));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(s => s.ServiceCategoryId == request.CategoryId);
        }

        if (request.ActivityTypeId.HasValue)
        {
            query = query.Where(s => s.ServiceActivityTypeId == request.ActivityTypeId);
        }

        if (request.IsFeatured.HasValue)
        {
            query = query.Where(s => s.IsFeatured == request.IsFeatured);
        }

        query = query.OrderBy(s => s.SortOrder);

        var paged = await PagedResult<Service>.CreateAsync(query, request.Page, request.PageSize, cancellationToken);

        var items = paged.Items.Select(service =>
        {
            var translation = TranslationSelector.Pick(service.Translations, request.Lang);
            var audienceNames = service.Audiences
                .Select(a => TranslationSelector.Pick(a.Translations, request.Lang)?.Name)
                .Where(name => name is not null)
                .Select(name => name!)
                .ToList();
            var audienceSlugs = service.Audiences.Select(a => a.Slug.Value).ToList();
            var categoryName = service.ServiceCategory is null
                ? null
                : TranslationSelector.Pick(service.ServiceCategory.Translations, request.Lang)?.Name;
            var activityName = service.ServiceActivityType is null
                ? null
                : TranslationSelector.Pick(service.ServiceActivityType.Translations, request.Lang)?.Name;
            return new ServiceListItemDto(service.Id, service.Slug.Value, translation?.Name ?? string.Empty, translation?.Description, (int)service.DeliveryType, service.IconId, service.IsFeatured, audienceNames, audienceSlugs, service.ServiceCategory?.Slug.Value, categoryName, service.ServiceActivityType?.Slug.Value, activityName);
        }).ToList();

        return new PagedResult<ServiceListItemDto> { Items = items, Page = paged.Page, PageSize = paged.PageSize, TotalCount = paged.TotalCount };
    }
}
