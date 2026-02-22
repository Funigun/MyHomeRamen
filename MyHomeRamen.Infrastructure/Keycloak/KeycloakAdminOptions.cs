namespace MyHomeRamen.Infrastructure.Keycloak;

public sealed class KeycloakAdminOptions
{
    public string BaseUrl { get; set; } = string.Empty;

    public string Realm { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Seconds the admin service-account token is cached before a new one is requested.
    /// Should be slightly shorter than the Keycloak client "Access Token Lifespan".
    /// Defaults to 270 s (4.5 min) which fits the Keycloak default of 300 s.
    /// </summary>
    public int TokenLifetimeSeconds { get; set; } = 270;
}
