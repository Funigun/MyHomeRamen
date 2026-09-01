using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Features.Common.Messaging;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.Features.Roles.Common;

namespace MyHomeRamen.Features.Identity.Features.Users.RegisterGuest;

public sealed record RegisterGuestCommand(RegisterGuestRequest Request) : ICommand<RegisterGuestResponse>;

public class RegisterGuestHandler(IIdentityDbContext dbContext, IMessagesService messagesService) : ICommandHandler<RegisterGuestCommand, RegisterGuestResponse>
{
    public async Task<RegisterGuestResponse> Handle(RegisterGuestCommand command, CancellationToken cancellationToken)
    {
        if (command.Request.ExistingGuestId.HasValue)
        {
            Guid? existing = await dbContext.User.Query().GetGuestIdByGuestIdAsync(command.Request.ExistingGuestId.Value, cancellationToken);
            if (existing.HasValue)
            {
                return new RegisterGuestResponse(existing.Value);
            }
        }

        Role guestRole = await dbContext.Role.Load().ByName(RoleConstants.Guest, cancellationToken)
                          ?? throw new InvalidOperationException("Guest role was not found.");

        User guest = User.CreateGuest();
        guest.AddRole(guestRole);
        dbContext.User.Add(guest);
        await dbContext.SaveChangesAsync(cancellationToken);

        await messagesService.PublishAsync(new GuestUserCreatedIntegrationEvent(guest.Id, guest.GuestId!.Value), cancellationToken);

        return guest.ToRegisterGuestResponse();
    }
}

internal static class Mappings
{
    extension(User user)
    {
        internal RegisterGuestResponse ToRegisterGuestResponse()
        {
            return new RegisterGuestResponse(user.GuestId!.Value);
        }
    }
}
