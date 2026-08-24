namespace SWA.Domain.Content.Services;

public sealed class ServiceChannel : LookupEntity
{
    public List<ServiceChannelTranslation> Translations { get; set; } = [];
    public List<Service> Services { get; set; } = [];
}
