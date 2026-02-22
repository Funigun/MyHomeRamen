using Microsoft.Extensions.Caching.Hybrid;
using MyHomeRamen.Api.Common.Cache;

namespace MyHomeRamen.Infrastructure.Cache;

internal sealed class CacheService(HybridCache hybridCache) : ICacheService
{
    public async Task<TResponse> GetOrSetAsync<TRequest, TResponse>(
        ICachePolicy<TRequest, TResponse> policy,
        Func<CancellationToken, ValueTask<TResponse>> factory,
        CancellationToken cancellationToken = default)
    {
        HybridCacheEntryOptions? options = null;

        if (policy.ExpirationTime.HasValue || policy.LocalExpirationTime.HasValue)
        {
            options = new HybridCacheEntryOptions
            {
                Expiration = policy.ExpirationTime,
                LocalCacheExpiration = policy.LocalExpirationTime,
            };
        }

        return await hybridCache.GetOrCreateAsync(
            policy.Key,
            factory,
            options,
            cancellationToken: cancellationToken);
    }

    public async Task RemoveAsync<TRequest, TResponse>(
        ICachePolicy<TRequest, TResponse> policy,
        CancellationToken cancellationToken = default)
    {
        await hybridCache.RemoveAsync(policy.Key, cancellationToken);
    }
}
