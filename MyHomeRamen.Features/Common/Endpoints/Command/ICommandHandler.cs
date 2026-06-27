namespace MyHomeRamen.Features.Common.Endpoints.Command;

public interface ICommandHandler<in TRequest>
           where TRequest : ICommand
{
    Task Handle(TRequest command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TRequest, TResponse>
           where TRequest : ICommand<TResponse>
{
    Task<TResponse> Handle(TRequest command, CancellationToken cancellationToken);
}
