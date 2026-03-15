using System.Text.Json.Serialization;

namespace MyHomeRamen.Infrastructure.Keycloak.Dto;

public sealed record KeycloakUserDto
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;

    public List<KeycloakCredentialDto> Credentials { get; set; } = [];
}
