namespace SWA.Application.Features.Public.Lookups;

public sealed record LookupDto(Guid Id, string Slug, string Name);

public enum LookupKey
{
    EventTypes,
    FaqCategories,
    ServiceAudiences,
    ServiceActivityTypes,
    ServiceCategories,
    ServiceChannels,
    DocumentCategories,
}
