using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using SWA.Application.Common.Interfaces;
using SWA.Infrastructure.Caching;
using SWA.Infrastructure.Persistence;
using SWA.Infrastructure.Persistence.Repositories;

namespace SWA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing 'DefaultConnection' connection string.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Missing 'Redis' connection string.");

        // AbortOnConnectFail = false: a Redis outage must fail per-call (catchable in
        // CachingBehavior), not throw once at startup/first-connect and take the app down.
        var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
        redisOptions.AbortOnConnectFail = false;

        services.AddStackExchangeRedisCache(options =>
        {
            options.ConfigurationOptions = redisOptions;
            options.InstanceName = "SWA:";
        });

        // Cache-group version comes from the same database (CacheGroupVersions, owned by the
        // CMS) instead of Redis, so the CMS never has to talk to Redis at all. This app is the
        // only one still using Redis, and only for the actual cached response bodies above.
        services.AddMemoryCache();
        services.AddScoped<ICacheVersionProvider, SqlCacheVersionProvider>();

        return services;
    }
}
