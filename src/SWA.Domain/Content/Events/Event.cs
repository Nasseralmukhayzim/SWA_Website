namespace SWA.Domain.Content.Events;

public sealed class Event : PublishableContent
{
    public DateTime StartsAtUtc { get; set; }
    public DateTime? EndsAtUtc { get; set; }
    public Guid? EventTypeId { get; set; }
    public EventType? EventType { get; set; }
    public Guid? ImageId { get; set; }
    public string? RegistrationUrl { get; set; }

    public List<EventTranslation> Translations { get; set; } = [];
}
