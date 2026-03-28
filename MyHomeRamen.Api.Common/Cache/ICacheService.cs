namespace MyHomeRamen.Api.Common.Cache;

public interface ICacheService
{
    Task<TResponse> GetOrSetAsync<TRequest, TResponse>(
        ICachePolicy<TRequest, TResponse> policy,
        Func<CancellationToken, ValueTask<TResponse>> factory,
        CancellationToken cancellationToken = default);

    Task RemoveAsync<TRequest, TResponse>(
        ICachePolicy<TRequest, TResponse> policy,
        CancellationToken cancellationToken = default);
}
