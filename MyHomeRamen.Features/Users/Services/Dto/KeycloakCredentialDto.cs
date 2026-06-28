namespace MyHomeRamen.Features.Users.Services.Dto;

public sealed record KeycloakCredentialDto
{
    public string Type { get; set; } = "password";

    public string Value { get; set; } = string.Empty;

    public bool Temporary { get; set; } = true;
}
