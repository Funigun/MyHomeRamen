namespace MyHomeRamen.Api.Common.Endpoint.Models;

public interface IRequest
{
}

public interface IRequest<TResponse> : IRequest
{
}
