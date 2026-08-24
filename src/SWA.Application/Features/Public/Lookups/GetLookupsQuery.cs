using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Domain.Content.Documents;
using SWA.Domain.Content.Events;
using SWA.Domain.Content.Faqs;
using SWA.Domain.Content.Services;

namespace SWA.Application.Features.Public.Lookups;

public sealed record GetLookupsQuery(LookupKey Key, string? Lang) : IRequest<IReadOnlyList<LookupDto>>;

public sealed class GetLookupsQueryHandler(
    IRepository<EventType> eventTypes,
    IRepository<FaqCategory> faqCategories,
    IRepository<ServiceAudience> serviceAudiences,
    IRepository<ServiceActivityType> serviceActivityTypes,
    IRepository<ServiceCategory> serviceCategories,
    IRepository<ServiceChannel> serviceChannels,
    IRepository<DocumentCategory> documentCategories) : IRequestHandler<GetLookupsQuery, IReadOnlyList<LookupDto>>
{
    public async Task<IReadOnlyList<LookupDto>> Handle(GetLookupsQuery request, CancellationToken cancellationToken) => request.Key switch
    {
        LookupKey.EventTypes => await Map(eventTypes, e => e.Translations.Select(t => (t.Name, t.Language)), request.Lang, cancellationToken),
        LookupKey.FaqCategories => await Map(faqCategories, c => c.Translations.Select(t => (t.Name, t.Language)), request.Lang, cancellationToken),
        LookupKey.ServiceAudiences => await Map(serviceAudiences, a => a.Translations.Select(t => (t.Name, t.Language)), request.Lang, cancellationToken),
        LookupKey.ServiceActivityTypes => await Map(serviceActivityTypes, a => a.Translations.Select(t => (t.Name, t.Language)), request.Lang, cancellationToken),
        LookupKey.ServiceCategories => await Map(serviceCategories, c => c.Translations.Select(t => (t.Name, t.Language)), request.Lang, cancellationToken),
        LookupKey.ServiceChannels => await Map(serviceChannels, c => c.Translations.Select(t => (t.Name, t.Language)), request.Lang, cancellationToken),
        LookupKey.DocumentCategories => await Map(documentCategories, c => c.Translations.Select(t => (t.Name, t.Language)), request.Lang, cancellationToken),
        _ => throw new ArgumentOutOfRangeException(nameof(request)),
    };

    private static async Task<IReadOnlyList<LookupDto>> Map<TEntity>(
        IRepository<TEntity> repository,
        Func<TEntity, IEnumerable<(string Name, string Language)>> namesSelector,
        string? lang,
        CancellationToken cancellationToken)
        where TEntity : Domain.Content.LookupEntity
    {
        var entities = await repository.Queryable()
            .Where(e => !e.IsDeleted && e.IsActive)
            .Include("Translations")
            .ToListAsync(cancellationToken);

        return entities
            .OrderBy(e => e.SortOrder).ThenBy(e => e.Slug.Value)
            .Select(entity =>
            {
                var names = namesSelector(entity).ToList();
                var requested = string.IsNullOrWhiteSpace(lang) ? TranslationSelector.DefaultLanguage : lang.Trim().ToLowerInvariant();
                var name = names.FirstOrDefault(n => n.Language.Equals(requested, StringComparison.OrdinalIgnoreCase)).Name
                    ?? names.FirstOrDefault(n => n.Language.Equals(TranslationSelector.DefaultLanguage, StringComparison.OrdinalIgnoreCase)).Name
                    ?? names.FirstOrDefault().Name
                    ?? string.Empty;
                return new LookupDto(entity.Id, entity.Slug.Value, name);
            })
            .ToList();
    }
}
