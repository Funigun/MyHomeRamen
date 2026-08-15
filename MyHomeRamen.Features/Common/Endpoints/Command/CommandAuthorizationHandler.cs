using MyHomeRamen.Features.Common.Endpoints.Policies;

namespace MyHomeRamen.Features.Common.Endpoints.Command;

public sealed class CommandAuthorizationHandler<TCommand>(IAuthorizationPolicy<TCommand> policy, ICommandHandler<TCommand> next) : ICommandHandler<TCommand>
              where TCommand : ICommand
{
    public async Task Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (!await policy.Authorize(command, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await next.Handle(command, cancellationToken);
    }
}

public sealed class CommandAuthorizationHandler<TCommand, TResponse>(IAuthorizationPolicy<TCommand> policy, ICommandHandler<TCommand, TResponse> next) : ICommandHandler<TCommand, TResponse>
              where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (!await policy.Authorize(command, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return await next.Handle(command, cancellationToken);
    }
}
