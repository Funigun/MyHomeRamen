using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MyHomeRamen.Infrastructure.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddCacheService(this IServiceCollection services)
    {
        services.AddHybridCache();
        services.TryAddSingleton<ICacheService, CacheService>();

        return services;
    }
}
