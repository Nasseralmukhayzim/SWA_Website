using SWA.Domain.Common;

namespace SWA.Domain.Content.Events;

public sealed class EventTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public required string Language { get; set; }
}
