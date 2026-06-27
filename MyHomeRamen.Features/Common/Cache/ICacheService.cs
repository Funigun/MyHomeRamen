namespace MyHomeRamen.Features.Common.Cache;

public interface ICacheService
{
    Task<TCached> GetOrSetAsync<TRequest, TCached>(
        ICachePolicy<TRequest, TCached> policy,
        TRequest request,
        Func<CancellationToken, ValueTask<TCached>> factory,
        CancellationToken cancellationToken);

    Task RemoveByKeyAsync(string key, CancellationToken cancellationToken);

    Task RemoveByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken);

    Task RemoveByRestaurantIdAsync(CancellationToken cancellationToken);
}
