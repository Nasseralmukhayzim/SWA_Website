using SWA.Domain.Enums;

namespace SWA.Domain.Content.Pages;

/// <summary>
/// Not an EF entity — serialized as JSON into PageTranslation.Sections (matches the CMS's
/// PageSection/PageSectionItem model; there are no PageSections/PageSectionItems tables).
/// </summary>
public sealed class PageSection
{
    public Guid? Id { get; set; }
    public SectionKind Kind { get; set; }
    public string? Heading { get; set; }
    public string? Intro { get; set; }
    public string? Body { get; set; }
    public List<PageSectionItem> Items { get; set; } = [];
}

public sealed class PageSectionItem
{
    public Guid? Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid? IconId { get; set; }
    public Guid? ImageId { get; set; }
    public string? LinkUrl { get; set; }
}
