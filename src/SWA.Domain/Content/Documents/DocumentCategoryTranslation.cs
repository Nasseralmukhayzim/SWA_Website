using SWA.Domain.Common;

namespace SWA.Domain.Content.Documents;

public sealed class DocumentCategoryTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid DocumentCategoryId { get; set; }
    public DocumentCategory DocumentCategory { get; set; } = null!;
    public required string Name { get; set; }
    public required string Language { get; set; }
}
