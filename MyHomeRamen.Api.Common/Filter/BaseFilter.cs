using Microsoft.AspNetCore.Http;
using MyHomeRamen.Api.Common.Cache;

namespace MyHomeRamen.Api.Common.Filter;

internal abstract class BaseFilter : IEndpointFilter
{
    protected Dictionary<string, string> cacheParameters = new()
    {
        { CacheConstants.UserIdCacheParameter, string.Empty },
        { CacheConstants.EntityIdCacheParameter, string.Empty },
    };

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Pre-processing
        object? result = await OnBeforeExecutionAsync(context);

        if (result != null)
        {
            return result;
        }

        // Execute the endpoint
        object? response = await next(context);

        // Post-processing
        return await OnAfterExecutionAsync(context, response);
    }

    protected virtual ValueTask<object?> OnBeforeExecutionAsync(EndpointFilterInvocationContext context)
    {
        return ValueTask.FromResult<object?>(null);
    }

    protected virtual ValueTask<object?> OnAfterExecutionAsync(EndpointFilterInvocationContext context, object? response)
    {
        return ValueTask.FromResult(response);
    }

    protected string SanitizeCacheKey(string key)
    {
        foreach (string parameter in cacheParameters.Keys)
        {
            key = key.Replace(parameter, cacheParameters[parameter]);
        }

        return key;
    }
}
