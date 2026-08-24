namespace SWA.Domain.Content.Services;

/// <summary>
/// The service families the e-services page filters by (networked water, non-networked water,
/// regulatory support, other). Distinct from <see cref="ServiceAudience"/>, which says who a
/// service is for rather than what kind of service it is.
/// </summary>
public sealed class ServiceCategory : LookupEntity
{
    public List<ServiceCategoryTranslation> Translations { get; set; } = [];
    public List<Service> Services { get; set; } = [];
}
