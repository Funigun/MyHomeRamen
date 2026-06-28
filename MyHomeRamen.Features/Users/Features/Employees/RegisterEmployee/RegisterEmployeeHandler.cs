using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Features.Users.Services;
using MyHomeRamen.Features.Users.Services.Dto;

namespace MyHomeRamen.Features.Users.Features.Employees.RegisterEmployee;

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

