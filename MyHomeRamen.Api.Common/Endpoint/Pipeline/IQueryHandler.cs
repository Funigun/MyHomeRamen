namespace MyHomeRamen.Api.Common.Endpoint.Pipeline;

public interface IQueryHandler<in TQuery, TResponse>
           where TQuery : IQuery<TResponse>
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
