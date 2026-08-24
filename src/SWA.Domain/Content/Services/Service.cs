using SWA.Domain.Content.Faqs;
using SWA.Domain.Enums;

namespace SWA.Domain.Content.Services;

public sealed class Service : PublishableContent
{
    public ServiceDeliveryType DeliveryType { get; set; }
    public Guid? IconId { get; set; }
    public string? SupportPhone { get; set; }
    public Guid? FaqCategoryId { get; set; }
    public FaqCategory? FaqCategory { get; set; }

    /// <summary>The service family this belongs to; a service sits in at most one.</summary>
    public Guid? ServiceCategoryId { get; set; }
    public ServiceCategory? ServiceCategory { get; set; }

    /// <summary>Where the service sits in the water value chain; null for non-activity services.</summary>
    public Guid? ServiceActivityTypeId { get; set; }
    public ServiceActivityType? ServiceActivityType { get; set; }
    public bool IsFeatured { get; set; }

    public List<ServiceTranslation> Translations { get; set; } = [];

    /// <summary>EF skip-navigation many-to-many; produces the ServiceAudienceLinks join table.</summary>
    public List<ServiceAudience> Audiences { get; set; } = [];

    /// <summary>EF skip-navigation many-to-many; produces the ServiceChannelLinks join table.</summary>
    public List<ServiceChannel> Channels { get; set; } = [];
}
