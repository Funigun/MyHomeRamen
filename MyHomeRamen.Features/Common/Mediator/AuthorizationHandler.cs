using MyHomeRamen.Features.Common.Endpoints.Policies;

namespace MyHomeRamen.Features.Common.Mediator;

public sealed class AuthorizationHandler<TRequest, TResponse>(IAuthorizationPolicy<TRequest> policy, IRequestHandler<TRequest, TResponse> next) : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (!await policy.Authorize(request, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return await next.Handle(request, cancellationToken);
    }
}
