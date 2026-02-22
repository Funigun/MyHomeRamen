namespace MyHomeRamen.Infrastructure.Keycloak;

public interface IKeycloakAdminService
{
    Task CreateUserAsync(KeycloakUserRepresentation user, CancellationToken cancellationToken = default);
}
