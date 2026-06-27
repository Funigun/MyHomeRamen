using FluentValidation;
using FluentValidation.Results;
using MyHomeRamen.Features.Common.Exceptions;

namespace MyHomeRamen.Features.Common.Endpoints.Command;

public sealed class CommandValidationHandler<TCommand>(IValidator<TCommand>? validator, ICommandHandler<TCommand> next) : ICommandHandler<TCommand>
       where TCommand : ICommand
{
    public async Task Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            ValidationContext<TCommand>? validationContext = new(command);
            ValidationResult validationResult = await validator.ValidateAsync(validationContext, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw CustomValidationException.ValidationFailed("Validation failed", validationResult.Errors);
            }
        }

        await next.Handle(command, cancellationToken);
    }
}

public sealed class CommandValidationHandler<TCommand, TResponse>(IValidator<TCommand>? validator, ICommandHandler<TCommand, TResponse> next) : ICommandHandler<TCommand, TResponse>
              where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            ValidationContext<TCommand>? validationContext = new(command);
            ValidationResult validationResult = await validator.ValidateAsync(validationContext, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw CustomValidationException.ValidationFailed("Validation failed", validationResult.Errors);
            }
        }

        return await next.Handle(command, cancellationToken);
    }
}
