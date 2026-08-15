using System.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using MyHomeRamen.Features.Common.Cache;

namespace MyHomeRamen.Infrastructure.Cache;

internal sealed class CacheService(HybridCache hybridCache) : ICacheService
{
    public async Task<TCached> GetOrSetAsync<TCached>(CachePolicy policy, Func<CancellationToken, ValueTask<TCached>> factory, CancellationToken cancellationToken)
    {
        HybridCacheEntryOptions? options = null;

        using Activity? activity = CacheDiagnostics.ActivitySource.StartActivity("CacheService.GetOrSet");

        options = new HybridCacheEntryOptions
        {
            Expiration = policy.DistributedExpirationTime ?? TimeSpan.FromMilliseconds(1),
            LocalCacheExpiration = policy.LocalExpirationTime ?? TimeSpan.FromMilliseconds(1),
        };

        string key = $"{policy.Module}_{policy.Key}";

        List<string> tags = policy.Tags.Select(tag => $"{policy.Module}_{tag}").ToList();

        TCached result = await hybridCache.GetOrCreateAsync
                        (
                            key, 
                            async (cancellationToken) =>
                            {
                                TCached results = await factory(cancellationToken);

                                if (results is not null && results.ToString() is not null)
                                {
                                    string tag = $"{policy.Module}_{results}";
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
        await hybridCache.RemoveAsync(key, cancellationToken);
    }

    public async Task RemoveByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken)
    {
        await hybridCache.RemoveByTagAsync(tags.ToList(), cancellationToken);
    }
}
