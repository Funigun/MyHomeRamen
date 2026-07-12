using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Features.Identity.Services;

public interface IKeycloakAdminService
{
    Task<string> CreateUserAsync(KeycloakUserDto user, string roleName, CancellationToken cancellationToken);

    Task<IEnumerable<KeycloakRoleDto>> GetAvailableRoles(CancellationToken cancellationToken);

    Task<IEnumerable<KeycloakUserDto>> GetEmployees(CancellationToken cancellationToken);
}
