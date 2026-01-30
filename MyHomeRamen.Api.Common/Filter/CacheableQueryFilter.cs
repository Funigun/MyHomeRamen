using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Exceptions;

namespace MyHomeRamen.Api.Common.Filter;

internal sealed class CacheableQueryFilter<TRequest, TResponse>(ICachePolicy<TRequest, TResponse> request, HybridCache hybridCache, ICurrentUser currentUser) : BaseFilter
{
    protected override async ValueTask<object?> OnBeforeExecutionAsync(EndpointFilterInvocationContext context)
    {
        object? endpointRequest = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TRequest));

        if (endpointRequest == null)
        {
            throw CustomValidationException.ValidationFailed($"Unable to find ICacheableQuery for {typeof(TRequest).BaseType?.FullName ?? typeof(TRequest).FullName}", []);
        }

        if (string.IsNullOrEmpty(request.Key))
        {
            throw CustomValidationException.ValidationFailed("Cache key cannot be null or empty", []);
        }

        cacheParameters[CacheConstants.UserIdCacheParameter] = $"{currentUser.Id}";
        cacheParameters[CacheConstants.EntityIdCacheParameter] = endpointRequest is IRequestId id ? id.Id.ToString() : string.Empty;

        await hybridCache.GetOrCreateAsync(
            key: SanitizeCacheKey(request.Key),
            factory: async entry =>
            {
                return await Task.FromResult(request);
            });

        return await base.OnBeforeExecutionAsync(context);
    }

    protected override async ValueTask<object?> OnAfterExecutionAsync(EndpointFilterInvocationContext context, object? response)
    {
        object? endpointRequest = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TRequest));

        cacheParameters[CacheConstants.UserIdCacheParameter] = $"{currentUser.Id}";
        cacheParameters[CacheConstants.EntityIdCacheParameter] = endpointRequest is IRequestId id ? id.Id.ToString() : string.Empty;

        await hybridCache.SetAsync(
            key: SanitizeCacheKey(request.Key),
            value: response,
            options: new HybridCacheEntryOptions
            {
                Expiration = request.ExpirationTime,
                LocalCacheExpiration = request.LocalExpirationTime
            });

        return await base.OnAfterExecutionAsync(context, response);
    }
}
