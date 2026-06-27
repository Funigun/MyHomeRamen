using MyHomeRamen.Features.Common.Cache;

namespace MyHomeRamen.Infrastructure.Keycloak;

internal sealed class KeycloakAdminTokenCachePolicy : ICachePolicy<KeycloakAdminOptions, string>
{
    internal KeycloakAdminTokenCachePolicy(int tokenLifetimeSeconds) =>
        ExpirationTime = TimeSpan.FromSeconds(tokenLifetimeSeconds);

    public string GetKey(KeycloakAdminOptions request) => "keycloak_admin_access_token";

    public TimeSpan? LocalExpirationTime => null;

    public TimeSpan? ExpirationTime { get; }

    public IEnumerable<string> Tags => ["keycloak_admin_token"];
}
