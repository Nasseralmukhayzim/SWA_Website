namespace SWA.Domain.Content.Pages;

public sealed class Page : PublishableContent
{
    public Guid? ParentId { get; set; }
    public Page? Parent { get; set; }
    public Guid? HeroImageId { get; set; }
    public bool ShowInNavigation { get; set; }
    public int ViewCount { get; set; }

    public List<PageTranslation> Translations { get; set; } = [];
}
