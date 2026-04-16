using Microsoft.Extensions.Caching.Hybrid;
using MyHomeRamen.Api.Common.Cache;

namespace MyHomeRamen.Infrastructure.Cache;

internal sealed class CacheService(HybridCache hybridCache) : ICacheService
{
    public async Task<TCached> GetOrSetAsync<TRequest, TCached>(
        ICachePolicy<TRequest, TCached> policy,
        TRequest request,
        Func<CancellationToken, ValueTask<TCached>> factory,
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
            policy.GetKey(request),
            factory,
            options,
            cancellationToken: cancellationToken);
    }

    public async Task RemoveAsync<TRequest, TCached>(
        ICachePolicy<TRequest, TCached> policy,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        await hybridCache.RemoveAsync(policy.GetKey(request), cancellationToken);
    }
}
