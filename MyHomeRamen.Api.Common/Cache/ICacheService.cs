namespace MyHomeRamen.Api.Common.Cache;

public interface ICacheService
{
    /// <summary>
    /// Retrieves a cached value identified by the given policy and request, or creates and caches
    /// a new value using the provided factory if no valid cache entry exists.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request used to determine the cache key and tags.</typeparam>
    /// <typeparam name="TCached">The type of the value to cache.</typeparam>
    /// <param name="policy">The cache policy that defines the cache key, tags, and expiration settings.</param>
    /// <param name="request">The request instance used to generate the cache key via the policy.</param>
    /// <param name="factory">
    /// An asynchronous factory delegate invoked to produce the value when no valid cache entry exists.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="Task{TCached}"/> representing the asynchronous operation,
    /// containing the cached or newly created value.
    /// </returns>
    Task<TCached> GetOrSetAsync<TRequest, TCached>(
        ICachePolicy<TRequest, TCached> policy,
        TRequest request,
        Func<CancellationToken, ValueTask<TCached>> factory,
        CancellationToken cancellationToken);

    /// <summary>
    /// Cache invalidation designed to use within feature handlers.
    /// Removes the cache entry associated with the specified key, scoped to the current restaurant.
    /// The restaurant identifier is automatically appended to the provided key.
    /// </summary>
    /// <param name="key">The cache key suffix identifying the entry to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous remove operation.</returns>
    Task RemoveByKeyAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Cache invalidation designed to use for admin/maintenance operations.
    /// Removes all cache entries associated with the specified tags, scoped to the current restaurant.
    /// The restaurant identifier is automatically appended to each tag.
    /// </summary>
    /// <param name="tags">A collection of tag names identifying the cache entries to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous remove operation.</returns>
    Task RemoveByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken);

    /// <summary>
    /// Cache invalidation designed to use for admin operations.
    /// Removes all cache entries belonging to the current restaurant by invalidating
    /// the restaurant-scoped tag.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous remove operation.</returns>
    Task RemoveByRestaurantIdAsync(CancellationToken cancellationToken);
}
