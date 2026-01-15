using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MyHomeRamen.Api.Common.Authorization;

namespace MyHomeRamen.Api.Common.Filter;

internal sealed class AuthorizationFilter<TRequest>(IAuthorizationPolicy<TRequest> authorizationPolicy) : BaseFilter
{
    protected override async ValueTask<object?> OnBeforeExecutionAsync(EndpointFilterInvocationContext context)
    {
        object? request = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TRequest));

        if (request == null)
        {
            List<ValidationFailure> error = [new(string.Empty, "Invalid request format")];
            throw new ValidationException(error);
        }

        return !await authorizationPolicy.IsAuthorized((TRequest)request!)
             ? throw new UnauthorizedAccessException("You are not authorized to perform this action.")
             : null;
    }
}
