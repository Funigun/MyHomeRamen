using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Common.Configurations;

namespace MyHomeRamen.Infrastructure.Cache;

internal sealed class CacheService(HybridCache hybridCache, IServiceScopeFactory scopeFactory) : ICacheService
{
    public async Task<TCached> GetOrSetAsync<TRequest, TCached>(
        ICachePolicy<TRequest, TCached> policy,
        TRequest request,
        Func<CancellationToken, ValueTask<TCached>> factory,
        CancellationToken cancellationToken)
    {
        RestaurantConfigurationProvider restaurantConfigurationProvider = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<RestaurantConfigurationProvider>();

        HybridCacheEntryOptions? options = null;

        if (policy.ExpirationTime.HasValue || policy.LocalExpirationTime.HasValue)
        {
            options = new HybridCacheEntryOptions
            {
                Expiration = policy.ExpirationTime,
                LocalCacheExpiration = policy.LocalExpirationTime,
            };
        }

        string restaurantId = restaurantConfigurationProvider.RestaurantId.ToString();
        string key = $"{restaurantId}_{policy.GetKey(request)}";
        List<string> tags = policy.Tags.Select(tag => $"{restaurantId}_{tag}").ToList();
        tags.Add(restaurantId);

        return await hybridCache.GetOrCreateAsync(
            key,
            factory,
            options,
            tags,
            cancellationToken: cancellationToken);
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
