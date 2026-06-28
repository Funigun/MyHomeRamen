using MyHomeRamen.Common.Contracts.Users.Roles.Responses;
using MyHomeRamen.Features.Users.Services.Dto;

namespace MyHomeRamen.Features.Users.Features.Roles.GetAvailableRoles;

public static class Mappings
{
    public static GetAvailableRolesResponse ToResponse(this IEnumerable<KeycloakRoleDto> roles)
        => new(roles.Select(r => new RoleDto(r.Id, r.Name, r.Description)));
}

