namespace MyHomeRamen.Features.Common.Endpoints.Policies;

public interface IAuthorizationPolicy<TRequest>
{
    Task<bool> Authorize(TRequest request, CancellationToken cancellationToken);
}
