using SWA.Domain.Common;

namespace SWA.Domain.Content.Documents;

public sealed class DocumentTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Document Document { get; set; } = null!;
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid? FileId { get; set; }
    public string? ExternalFileUrl { get; set; }
    public required string Language { get; set; }
}
