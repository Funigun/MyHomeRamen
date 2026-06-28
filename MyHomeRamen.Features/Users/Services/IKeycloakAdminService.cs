using MyHomeRamen.Features.Users.Services.Dto;

namespace MyHomeRamen.Features.Users.Services;

public interface IKeycloakAdminService
{
    Task<string> CreateUserAsync(KeycloakUserDto user, string roleName, CancellationToken cancellationToken = default);

    Task<IEnumerable<KeycloakRoleDto>> GetAvailableRoles(CancellationToken cancellationToken = default);

    Task<IEnumerable<KeycloakUserDto>> GetEmployees(CancellationToken cancellationToken = default);
}
