using FluentValidation;
using FluentValidation.Results;
using MyHomeRamen.Features.Common.Exceptions;

namespace MyHomeRamen.Features.Common.Mediator;

public sealed class ValidationHandler<TRequest, TResponse>(IValidator<TRequest>? validator, IRequestHandler<TRequest, TResponse> next) : IRequestHandler<TRequest, TResponse>
              where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            ValidationContext<TRequest> validationContext = new(request);
            ValidationResult validationResult = await validator.ValidateAsync(validationContext, cancellationToken);

            if (!validationResult.IsValid)
            {
                ValidationFailure? conflictError = validationResult.Errors.FirstOrDefault(error => error.ErrorCode == "Conflict");

                if (conflictError is not null)
                {
                    throw new ConflictException(conflictError.ErrorMessage);
                }

                throw CustomValidationException.ValidationFailed("Validation failed", validationResult.Errors);
            }
        }

        return await next.Handle(request, cancellationToken);
    }
}
