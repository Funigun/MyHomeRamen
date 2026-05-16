namespace MyHomeRamen.Api.Users.Features.Roles.GetAvailableRoles.Models;

public sealed record GetAvailableRolesResponse(IEnumerable<RoleDto> Roles);
