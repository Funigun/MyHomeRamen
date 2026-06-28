namespace MyHomeRamen.Features.Users.Services.Dto;

public sealed record KeycloakClientDto
{
    public string Id { get; init; } = string.Empty;

    public string ClientId { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;
}
