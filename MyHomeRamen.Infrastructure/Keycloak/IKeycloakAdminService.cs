using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Infrastructure.Keycloak;

public interface IKeycloakAdminService
{
    Task CreateUserAsync(KeycloakUserDto user, CancellationToken cancellationToken = default);

    Task<IEnumerable<KeycloakRoleDto>> GetAvailableRoles(CancellationToken cancellationToken = default);

    Task<IEnumerable<KeycloakUserDto>> GetEmployees(CancellationToken cancellationToken = default);
}
