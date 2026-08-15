using MyHomeRamen.Features.Common.Endpoints.Policies;

namespace MyHomeRamen.Features.Common.Endpoints.Query;

public sealed class QueryAuthorizationHandler<TQuery, TResponse>(IAuthorizationPolicy<TQuery> policy, IQueryHandler<TQuery, TResponse> next) : IQueryHandler<TQuery, TResponse>
              where TQuery : IQuery<TResponse>
{
    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        if (!await policy.Authorize(query, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return await next.Handle(query, cancellationToken);
    }
}
