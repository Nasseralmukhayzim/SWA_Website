namespace SWA.Application.Common.Content;

/// <summary>Controls which rows the anonymous public API is allowed to surface.</summary>
public sealed class PublicContentOptions
{
    public const string SectionName = "PublicContent";

    /// <summary>
    /// Also serve content that has not reached Published status. Needed when pointing at a CMS
    /// database whose editorial workflow has not been run yet; must stay false for a real
    /// public deployment, where draft content must never leave the CMS.
    /// </summary>
    public bool IncludeUnpublished { get; set; }
}
