namespace MyHomeRamen.Api.Common.Endpoint.Pipeline;

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
