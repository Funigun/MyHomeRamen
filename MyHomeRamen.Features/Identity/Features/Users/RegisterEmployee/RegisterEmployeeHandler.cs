using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Identity.Services.Dto;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.RegisterEmployee;

public sealed class RegisterEmployeeHandler(IKeycloakAdminService keycloakAdminService, IIdentityDbContext usersDbContext) : ICommandHandler<RegisterEmployeeCommand>
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

        usersDbContext.User.Add(user);
        await usersDbContext.SaveChangesAsync(cancellationToken);
    }
}

