namespace MyHomeRamen.Api.Common.Cache;

public interface ICacheService
{
    Task<TCached> GetOrSetAsync<TRequest, TCached>(
        ICachePolicy<TRequest, TCached> policy,
        TRequest request,
        Func<CancellationToken, ValueTask<TCached>> factory,
        CancellationToken cancellationToken = default);

    Task RemoveAsync<TRequest, TCached>(
        ICachePolicy<TRequest, TCached> policy,
        TRequest request,
        CancellationToken cancellationToken = default);
}
