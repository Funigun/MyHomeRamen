using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Features.Common.Messaging;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Api.Users.Features.Account.Register;

public class RegisterHandler(IKeycloakAdminService keycloakAdminService, IUsersDbContext usersDbContext, IMessagesService messagesService) : ICommandHandler<RegisterCommand>
{
    public async Task Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        KeycloakUserDto keycloakUser = command.Request.ToKeycloakUserDto();

        string keycloakUserId = await keycloakAdminService.CreateUserAsync(keycloakUser, RoleConstants.Customer, cancellationToken);

        User user = command.Request.ToUserDto(keycloakUserId, RoleConstants.Customer);

        usersDbContext.Users.Add(user);
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
