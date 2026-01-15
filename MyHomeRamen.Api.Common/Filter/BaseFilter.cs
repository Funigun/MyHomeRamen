using Microsoft.AspNetCore.Http;

namespace MyHomeRamen.Api.Common.Filter;

internal abstract class BaseFilter : IEndpointFilter
{
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
}
