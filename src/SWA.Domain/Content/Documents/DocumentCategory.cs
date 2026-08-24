using SWA.Domain.Enums;

namespace SWA.Domain.Content.Documents;

public sealed class DocumentCategory : LookupEntity
{
    public DocumentSection Section { get; set; }
    public List<DocumentCategoryTranslation> Translations { get; set; } = [];
}
