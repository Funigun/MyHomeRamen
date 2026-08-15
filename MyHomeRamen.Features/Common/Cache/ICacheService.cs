namespace MyHomeRamen.Features.Common.Cache;

public interface ICacheService
{
    Task<TCached> GetOrSetAsync<TCached>(CachePolicy policy, Func<CancellationToken, ValueTask<TCached>> factory, CancellationToken cancellationToken);

    Task RemoveByKeyAsync(string key, CancellationToken cancellationToken);

    Task RemoveByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken);
}
