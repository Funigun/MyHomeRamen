using MyHomeRamen.Common.Contracts.Users.Roles.Responses;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Api.Users.Features.Roles.GetAvailableRoles;

public static class Mappings
{
    public static GetAvailableRolesResponse ToResponse(this IEnumerable<KeycloakRoleDto> roles)
        => new(roles.Select(r => new RoleDto(r.Id, r.Name, r.Description)));
}
