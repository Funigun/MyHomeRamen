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
