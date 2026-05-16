using FluentValidation;
using FluentValidation.Results;
using MyHomeRamen.Api.Common.Exceptions;

namespace MyHomeRamen.Api.Common.Endpoint.Pipeline;

public sealed class QueryValidationHandler<TQuery, TResponse>(IValidator<TQuery>? validator, IQueryHandler<TQuery, TResponse> next) : IQueryHandler<TQuery, TResponse>
              where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            ValidationContext<TQuery>? validationContext = new(query);
            ValidationResult validationResult = await validator.ValidateAsync(validationContext, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw CustomValidationException.ValidationFailed("Validation failed", validationResult.Errors);
            }
        }

        return await next.Handle(query, cancellationToken);
    }
}
