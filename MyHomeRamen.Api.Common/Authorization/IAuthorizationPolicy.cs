namespace MyHomeRamen.Api.Common.Authorization;

public interface IAuthorizationPolicy<TRequest>
{
    Task<bool> IsAuthorized(TRequest request);
}
