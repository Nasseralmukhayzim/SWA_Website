using SWA.Domain.Common;

namespace SWA.Domain.Content.Pages;

public sealed class PageTranslation : ITranslation
{
    public Guid Id { get; set; }
    public Guid PageId { get; set; }
    public Page Page { get; set; } = null!;
    public required string Title { get; set; }
    public string? Summary { get; set; }
    public required string Body { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public required string Language { get; set; }
    public List<PageSection>? Sections { get; set; }
}
