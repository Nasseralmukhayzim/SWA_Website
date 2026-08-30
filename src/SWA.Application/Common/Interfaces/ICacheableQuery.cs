namespace SWA.Application.Common.Interfaces;

public interface ICacheableQuery
{
    /// <summary>The content group this query reads from (e.g. "Services", "Lookups") — bumped by the CMS on save to invalidate every cached key in the group at once.</summary>
    string CacheGroup { get; }
    string CacheKey { get; }
    TimeSpan CacheDuration { get; }
}
