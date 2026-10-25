namespace MyHomeRamen.Features.Common.Mediator;

public interface IRequest<out TResponse>
{
}

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand : IRequest<Unit>
{
}

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
