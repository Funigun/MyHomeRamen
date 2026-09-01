using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Features.Identity.Services;

public interface IKeycloakAdminService
{
    Task<string> CreateUserAsync(KeycloakUserDto user, CancellationToken cancellationToken);
}
