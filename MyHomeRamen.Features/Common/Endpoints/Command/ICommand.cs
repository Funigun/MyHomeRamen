namespace MyHomeRamen.Features.Common.Endpoints.Command;

public interface ICommand
{
}

public interface ICommand<TResponse> : ICommand
{
}
