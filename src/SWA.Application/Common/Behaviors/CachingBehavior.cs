using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SWA.Application.Common.Interfaces;

namespace SWA.Application.Common.Behaviors;

/// <summary>
/// Caching is an optimization, not a dependency the site should go down over — every step here
/// (resolving the cache-group version, reading/writing the cached response) is individually
/// guarded, so a failure anywhere just means this request falls through to the database, exactly
/// like caching was never wired in.
/// </summary>
public sealed class CachingBehavior<TRequest, TResponse>(
    IDistributedCache cache,
    ICacheVersionProvider versionProvider,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheable)
        {
            return await next(cancellationToken);
        }

        var key = await TryBuildKeyAsync(cacheable, cancellationToken);
        var cached = key is null ? null : await TryGetAsync(key, cancellationToken);
        if (cached is not null)
        {
            return JsonSerializer.Deserialize<TResponse>(cached)!;
        }

        var response = await next(cancellationToken);

        if (key is not null)
        {
            await TrySetAsync(key, response, cacheable.CacheDuration, cancellationToken);
        }

        return response;
    }

    private async Task<string?> TryBuildKeyAsync(ICacheableQuery cacheable, CancellationToken cancellationToken)
    {
        try
        {
            var version = await versionProvider.GetVersionAsync(cacheable.CacheGroup, cancellationToken);
            return $"{cacheable.CacheGroup}:v{version}:{cacheable.CacheKey}";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to resolve cache version for group {CacheGroup}; bypassing cache.", cacheable.CacheGroup);
            return null;
        }
    }

    private async Task<string?> TryGetAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            return await cache.GetStringAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis unavailable while reading cache key {CacheKey}; bypassing cache.", key);
            return null;
        }
    }

    private async Task TrySetAsync(string key, TResponse response, TimeSpan duration, CancellationToken cancellationToken)
    {
        try
        {
            await cache.SetStringAsync(
                key,
                JsonSerializer.Serialize(response),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = duration },
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis unavailable while writing cache key {CacheKey}; response was not cached.", key);
        }
    }
}
