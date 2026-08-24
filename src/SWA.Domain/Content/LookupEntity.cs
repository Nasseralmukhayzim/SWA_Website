using SWA.Domain.Common;

namespace SWA.Domain.Content;

/// <summary>Base for named reference lists (EventTypes, FaqCategories, ServiceAudiences, ServiceChannels, DocumentCategories).</summary>
public abstract class LookupEntity : AuditableEntity
{
    public Slug Slug { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}
