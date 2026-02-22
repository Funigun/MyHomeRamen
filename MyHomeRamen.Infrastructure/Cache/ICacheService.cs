using MyHomeRamen.Api.Common.Cache;

namespace MyHomeRamen.Infrastructure.Cache;

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
