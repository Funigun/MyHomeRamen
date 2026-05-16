using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Common.Messaging;
using MyHomeRamen.Api.Users.Features.Account.RegisterGuest.Models;
using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Users.Features.Account.RegisterGuest;

public class RegisterGuestHandler(IUsersDbContext dbContext, IMessagesService messagesService) : IRequestHandler<RegisterGuestRequest, RegisterGuestResponse>
{
    public async Task<RegisterGuestResponse> Handle(RegisterGuestRequest request, CancellationToken cancellationToken)
    {
        if (request.ExistingGuestId.HasValue)
        {
            Guid? existing = await dbContext.Users.GetGuestIdByGuestIdAsync(request.ExistingGuestId.Value, cancellationToken);
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
