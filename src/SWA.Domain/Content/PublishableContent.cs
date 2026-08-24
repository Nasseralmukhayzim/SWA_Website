using SWA.Domain.Common;
using SWA.Domain.Enums;

namespace SWA.Domain.Content;

/// <summary>Base for the six top-level content types (Pages, NewsArticles, Events, Faqs, Services, Documents).</summary>
public abstract class PublishableContent : AuditableEntity
{
    public Slug Slug { get; set; }
    public int SortOrder { get; set; }
    public ContentStatus Status { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public Guid? PublishedBy { get; set; }
    public DateTime? DeletionRequestedAtUtc { get; set; }
    public Guid? DeletionRequestedBy { get; set; }
    public DeletionRequestStatus DeletionStatus { get; set; }
}
