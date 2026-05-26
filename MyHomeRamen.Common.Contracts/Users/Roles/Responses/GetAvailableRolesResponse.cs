namespace MyHomeRamen.Common.Contracts.Users.Roles.Responses;

public sealed record GetAvailableRolesResponse(IEnumerable<RoleDto> Roles);

public sealed record RoleDto(string Id, string Name, string Description);
