namespace MyHomeRamen.Api.Common.Endpoint.Pipeline;

public interface ICommand
{
}

public interface ICommand<TResponse> : ICommand
{
}
