namespace MyHomeRamen.Features.Common.Endpoints.Query;

public interface IQueryHandler<in TQuery, TResponse>
           where TQuery : IQuery<TResponse>
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}
