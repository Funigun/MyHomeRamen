namespace MyHomeRamen.Identity.Api.Features.Admin.GetAvailableRoles.Models;

public sealed record GetAvailableRolesResponse(IEnumerable<RoleDto> Roles);
