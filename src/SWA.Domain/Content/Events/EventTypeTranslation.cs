using SWA.Domain.Common;

namespace SWA.Domain.Content.Events;

public sealed class EventTypeTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid EventTypeId { get; set; }
    public EventType EventType { get; set; } = null!;
    public required string Name { get; set; }
    public required string Language { get; set; }
}
