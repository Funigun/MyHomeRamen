using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Common.Endpoint;

namespace MyHomeRamen.Api.Common.Filter;

internal sealed class CacheableCommandFilter<TRequest, TResponse>(ICachePolicy<TRequest, TResponse> request, HybridCache hybridCache, ICurrentUser currentUser) : BaseFilter
{
    protected override async ValueTask<object?> OnAfterExecutionAsync(EndpointFilterInvocationContext context, object? response)
    {
        object? endpointRequest = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TRequest));

        cacheParameters[CacheConstants.UserIdCacheParameter] = $"{currentUser.Id}";
        cacheParameters[CacheConstants.EntityIdCacheParameter] = endpointRequest is IRequestId id ? id.Id.ToString() : string.Empty;

        await hybridCache.RemoveAsync(key: SanitizeCacheKey(request.Key));

        return await base.OnAfterExecutionAsync(context, response);
    }
}
