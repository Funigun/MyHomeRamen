using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MyHomeRamen.Api.Common.Exceptions;

namespace MyHomeRamen.Api.Common.Filter;

internal sealed class ValidationFilter<TRequest>(IValidator<TRequest> validator) : BaseFilter
{
    protected override async ValueTask<object?> OnBeforeExecutionAsync(EndpointFilterInvocationContext context)
    {
        object? request = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TRequest));

        if (request == null)
        {
            List<ValidationFailure> error = [new(string.Empty, "Invalid request format")];
            throw new ValidationException(error);
        }

        ValidationContext<TRequest>? validationContext = new((TRequest)request);
        ValidationResult validationResult = await validator.ValidateAsync(validationContext);

        if (!validationResult.IsValid)
        {
            throw CustomValidationException.ValidationFailed("Validation failed", validationResult.Errors);
        }

        return null;
    }
}
