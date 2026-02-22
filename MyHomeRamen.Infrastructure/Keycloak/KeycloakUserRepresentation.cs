using System.Text.Json.Serialization;

namespace MyHomeRamen.Infrastructure.Keycloak;

public sealed class KeycloakUserRepresentation
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("credentials")]
    public List<KeycloakCredentialRepresentation> Credentials { get; set; } = [];
}
