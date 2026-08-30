using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SWA.Application.Common.Interfaces;
using SWA.Infrastructure.Persistence;

namespace SWA.Infrastructure.Caching;

/// <summary>
/// Reads the per-group version the CMS bumps (CacheGroupVersions.UpdatedAtUtc) from the same
/// database, instead of from Redis — the CMS never talks to Redis at all, so this is the only
/// side of the invalidation handshake, and it stays entirely inside infrastructure this app
/// already depends on for every request. A short in-memory layer keeps this from adding a SQL
/// round-trip to every cached request, which would defeat the point of caching.
/// </summary>
internal sealed class SqlCacheVersionProvider(ApplicationDbContext db, IMemoryCache memoryCache) : ICacheVersionProvider
{
    private static readonly TimeSpan LocalCacheDuration = TimeSpan.FromSeconds(5);

    public async Task<long> GetVersionAsync(string group, CancellationToken cancellationToken)
    {
        var version = await memoryCache.GetOrCreateAsync($"cache-version:{group}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = LocalCacheDuration;

            var updatedAtUtc = await db.CacheGroupVersions
                .Where(v => v.GroupName == group)
                .Select(v => v.UpdatedAtUtc)
                .FirstOrDefaultAsync(cancellationToken);

            return updatedAtUtc.Ticks;
        });

        return version;
    }
}
