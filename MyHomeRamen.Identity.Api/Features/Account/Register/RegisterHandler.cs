using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.Register.Models;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Identity.Api.Features.Account.Register;

public class RegisterHandler(IKeycloakAdminService keycloakAdminService, IUsersDbContext usersDbContext) : IRequestHandler<RegisterRequest>
{
    public async Task Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        KeycloakUserDto keycloakUser = request.ToKeycloakUserDto();

        string keycloakUserId = await keycloakAdminService.CreateUserAsync(keycloakUser, RoleConstants.Customer, cancellationToken);

        User user = request.ToUserDto(keycloakUserId, RoleConstants.Customer);

        usersDbContext.Users.Add(user);
        await usersDbContext.SaveChangesAsync(cancellationToken);
    }
}
