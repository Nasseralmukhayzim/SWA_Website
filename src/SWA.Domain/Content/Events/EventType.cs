using SWA.Domain.Content;

namespace SWA.Domain.Content.Events;

public sealed class EventType : LookupEntity
{
    public List<EventTypeTranslation> Translations { get; set; } = [];
}
