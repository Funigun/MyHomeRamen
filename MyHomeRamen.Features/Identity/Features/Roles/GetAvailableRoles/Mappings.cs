using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Features.Identity.Features.Roles.GetAvailableRoles;

public static class Mappings
{
    public static GetAvailableRolesResponse ToResponse(this IEnumerable<KeycloakRoleDto> roles)
        => new(roles.Select(r => new RoleDto(r.Id, r.Name, r.Description)));
}

