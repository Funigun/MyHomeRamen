namespace MyHomeRamen.Features.Identity.Services.Dto;

public sealed record KeycloakRoleDto
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}
