using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyHomeRamen.Infrastructure.Cache;
using NSubstitute;
using StackExchange.Redis;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common;

internal static class IdentityApiServicesExtensions
{
    internal static IServiceCollection ReconfigureCache(this IServiceCollection services)
    {
        services.RemoveAll<RedisCacheOptions>();
        services.RemoveAll<IConnectionMultiplexer>();
        services.RemoveAll<HybridCacheEntryOptions>();
        services.RemoveAll<HybridCache>();

        services.AddCacheService();

        return services;
    }

    internal static IServiceCollection ReplaceWithNoop<T>(this IServiceCollection services)
        where T : class
    {
        services.RemoveAll<T>();
        services.AddSingleton(Substitute.For<T>());
        return services;
    }
}
