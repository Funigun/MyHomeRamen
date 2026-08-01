using System.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Common.Configurations;

namespace MyHomeRamen.Infrastructure.Cache;

internal sealed class CacheService(HybridCache hybridCache, IServiceScopeFactory scopeFactory) : ICacheService
{
    public async Task<TCached> GetOrSetAsync<TCached>(CachePolicy policy, Func<CancellationToken, ValueTask<TCached>> factory, CancellationToken cancellationToken)
    {
        RestaurantConfigurationProvider restaurantConfigurationProvider = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<RestaurantConfigurationProvider>();
        HybridCacheEntryOptions? options = null;

        using Activity? activity = CacheDiagnostics.ActivitySource.StartActivity("CacheService.GetOrSet");

        if (policy.DistributedExpirationTime.HasValue || policy.LocalExpirationTime.HasValue)
        {
            options = new HybridCacheEntryOptions
            {
                Expiration = policy.DistributedExpirationTime,
                LocalCacheExpiration = policy.LocalExpirationTime,
            };
        }

        string restaurantId = restaurantConfigurationProvider.RestaurantId.ToString();
        string key = $"{restaurantId}_{policy.Module}_{policy.Key}";

        List<string> tags = policy.Tags.Select(tag => $"{restaurantId}_{policy.Module}_{tag}").ToList();

        tags.Add(restaurantId);

        TCached result = await hybridCache.GetOrCreateAsync
                        (
                            key, 
                            async (cancellationToken) =>
                            {
                                TCached results = await factory(cancellationToken);

                                if (results is not null && results.ToString() is not null)
                                {
                                    string tag = $"{restaurantId}_{policy.Module}_{results.ToString()!}";
                                    tags.Add(tag);
                                    activity?.AddTag("CacheService.ComputedKey", tag);
                                }

                                return results;
                            },
                            options,
                            tags,
                            cancellationToken
                        );

        return result;
    }

    public async Task RemoveByKeyAsync(string key, CancellationToken cancellationToken)
    {
        RestaurantConfigurationProvider restaurantConfigurationProvider = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<RestaurantConfigurationProvider>();

        string restaurantId = restaurantConfigurationProvider.RestaurantId.ToString();
        string fullKey = $"{restaurantId}_{key}";

        await hybridCache.RemoveAsync(fullKey, cancellationToken);
    }

    public async Task RemoveByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken)
    {
        RestaurantConfigurationProvider restaurantConfigurationProvider = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<RestaurantConfigurationProvider>();

        string restaurantId = restaurantConfigurationProvider.RestaurantId.ToString();
        List<string> fullTags = tags.Select(tag => $"{restaurantId}_{tag}").ToList();

        await hybridCache.RemoveByTagAsync(fullTags, cancellationToken);
    }

    public async Task RemoveByRestaurantIdAsync(CancellationToken cancellationToken)
    {
        RestaurantConfigurationProvider restaurantConfigurationProvider = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<RestaurantConfigurationProvider>();

        string restaurantId = restaurantConfigurationProvider.RestaurantId.ToString();

        await hybridCache.RemoveByTagAsync([restaurantId], cancellationToken);
    }
}
