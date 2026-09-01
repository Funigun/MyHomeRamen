namespace MyHomeRamen.Features.Common.Mediator;

public interface IRequest<out TResponse>
{
}

public interface IQuery<TResponse> : IRequest<TResponse>
{
}

public interface ICommand : IRequest<Unit>
{
}

public interface ICommand<TResponse> : IRequest<TResponse>
{
}
