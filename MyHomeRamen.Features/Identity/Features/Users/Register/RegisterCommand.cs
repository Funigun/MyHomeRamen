using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Features.Common.Messaging;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Identity.Services.Dto;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.Register;

public sealed record RegisterCommand(RegisterRequest Request) : ICommand;

public class RegisterHandler(IKeycloakAdminService keycloakAdminService, IIdentityDbContext usersDbContext, IMessagesService messagesService) : ICommandHandler<RegisterCommand>
{
    public async Task Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        KeycloakUserDto keycloakUser = command.Request.ToKeycloakUserDto();

        string keycloakUserId = await keycloakAdminService.CreateUserAsync(keycloakUser, RoleConstants.Customer, cancellationToken);

        Role role = await usersDbContext.Role.Specification().ByName(RoleConstants.Customer, cancellationToken);
        User user = command.Request.ToUserDto(keycloakUserId, role);

        usersDbContext.User.Add(user);
        await usersDbContext.SaveChangesAsync(cancellationToken);

        UserRegisteredIntegrationEvent integrationEvent = new(
            user.Id,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Email,
            user.Role);

        await messagesService.PublishAsync(integrationEvent, cancellationToken);
    }
}

