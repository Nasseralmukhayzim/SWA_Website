namespace SWA.Domain.Content.Services;

public sealed class ServiceAudience : LookupEntity
{
    public List<ServiceAudienceTranslation> Translations { get; set; } = [];
    public List<Service> Services { get; set; } = [];
}
