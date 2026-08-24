namespace SWA.Domain.Content.Services;

/// <summary>
/// Where a service sits in the water value chain — collection, treatment, distribution, transport,
/// storage, production, preliminary approvals. The e-services page filters by this under the
/// "نوع النشاط" heading. Orthogonal to <see cref="ServiceCategory"/>: a service has at most one of
/// each, and many services (complaints, calculators, platforms) have no activity at all.
/// </summary>
public sealed class ServiceActivityType : LookupEntity
{
    public List<ServiceActivityTypeTranslation> Translations { get; set; } = [];
    public List<Service> Services { get; set; } = [];
}
