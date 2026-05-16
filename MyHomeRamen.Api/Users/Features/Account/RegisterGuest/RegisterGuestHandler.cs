using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Common.Messaging;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Users.Features.Account.RegisterGuest;

public class RegisterGuestHandler(IUsersDbContext dbContext, IMessagesService messagesService) : IRequestHandler<RegisterGuestCommand, RegisterGuestResponse>
{
    public async Task<RegisterGuestResponse> Handle(RegisterGuestCommand command, CancellationToken cancellationToken)
    {
        if (command.Request.ExistingGuestId.HasValue)
        {
            Guid? existing = await dbContext.Users.GetGuestIdByGuestIdAsync(command.Request.ExistingGuestId.Value, cancellationToken);
            if (existing.HasValue)
            {
                return new RegisterGuestResponse(existing.Value);
            }
        }

        User guest = User.CreateGuest();
        dbContext.Users.Add(guest);
        await dbContext.SaveChangesAsync(cancellationToken);

        await messagesService.PublishAsync(new GuestUserCreatedIntegrationEvent(guest.Id, guest.GuestId!.Value), cancellationToken);

        return guest.ToRegisterGuestResponse();
    }
}
