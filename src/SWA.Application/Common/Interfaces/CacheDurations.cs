namespace SWA.Application.Common.Interfaces;

/// <summary>
/// Shared TTL for content whose Redis entry is actually invalidated by a CacheGroupVersions bump
/// on CMS save, not by expiry — this long duration is just a safety net, not the real
/// invalidation mechanism. Only use this for a CacheGroup the CMS is confirmed to stamp a
/// version for; a group it never touches (e.g. "Search") would go stale for the full duration
/// with nothing to invalidate it early.
/// </summary>
public static class CacheDurations
{
    public static readonly TimeSpan LongLived = TimeSpan.FromDays(30);
}
