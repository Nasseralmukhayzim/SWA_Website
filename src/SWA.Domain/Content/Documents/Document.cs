using SWA.Domain.Enums;

namespace SWA.Domain.Content.Documents;

public sealed class Document : PublishableContent
{
    public DocumentSection Section { get; set; }
    public Guid? CategoryId { get; set; }
    public DocumentCategory? Category { get; set; }
    public int? Year { get; set; }
    public Guid? CoverImageId { get; set; }

    public List<DocumentTranslation> Translations { get; set; } = [];
}
