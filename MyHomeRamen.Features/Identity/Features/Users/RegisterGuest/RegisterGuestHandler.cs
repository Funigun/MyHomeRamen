using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Features.Common.Messaging;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.Features.Identity.Features.Users.RegisterGuest;

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

        User guest = User.CreateGuest();
        dbContext.User.Add(guest);
        await dbContext.SaveChangesAsync(cancellationToken);

        await messagesService.PublishAsync(new GuestUserCreatedIntegrationEvent(guest.Id, guest.GuestId!.Value), cancellationToken);

        return guest.ToRegisterGuestResponse();
    }
}

