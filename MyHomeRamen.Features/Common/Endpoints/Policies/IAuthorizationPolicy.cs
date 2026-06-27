namespace MyHomeRamen.Features.Common.Endpoints.Policies;

public interface IAuthorizationPolicy<TRequest>
{
    Task<bool> IsAuthorized(TRequest request);
}
