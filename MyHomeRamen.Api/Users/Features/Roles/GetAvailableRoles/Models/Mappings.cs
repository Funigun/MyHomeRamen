using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Api.Users.Features.Roles.GetAvailableRoles.Models;

internal static class Mappings
{
    public static RoleDto ToDto(this KeycloakRoleDto role)
    {
        return new(role.Id, role.Name, role.Description);
    }

    public static GetAvailableRolesResponse ToResponse(this IEnumerable<KeycloakRoleDto> roles)
    {
        return new GetAvailableRolesResponse(roles.Select(r => r.ToDto()));
    }
}
