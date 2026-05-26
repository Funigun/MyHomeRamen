using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Api.Users.Features.Employees.RegisterEmployee;

public sealed class RegisterEmployeeHandler(IKeycloakAdminService keycloakAdminService, IUsersDbContext usersDbContext) : ICommandHandler<RegisterEmployeeCommand>
{
    public async Task Handle(RegisterEmployeeCommand command, CancellationToken cancellationToken)
    {
        KeycloakUserDto keycloakUser = command.Request.ToUserDto();

        string keycloakUserId = await keycloakAdminService.CreateUserAsync(keycloakUser, RoleConstants.Employee, cancellationToken);

        User user = User.Create(
            keycloakUserId,
            command.Request.Username,
            command.Request.FirstName,
            command.Request.LastName,
            command.Request.Email,
            command.Request.PhoneNumber,
            RoleConstants.Employee);

        usersDbContext.Users.Add(user);
        await usersDbContext.SaveChangesAsync(cancellationToken);
    }
}
