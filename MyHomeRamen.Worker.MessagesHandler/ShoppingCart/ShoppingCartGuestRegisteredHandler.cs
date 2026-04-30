using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Worker.Common;

namespace MyHomeRamen.Worker.MessagesHandler.ShoppingCart;

internal sealed class ShoppingCartGuestRegisteredHandler(IShoppingCartDbContext dbContext) : IIntegrationEventHandler<GuestUserCreatedIntegrationEvent>
{
    public async Task HandleAsync(GuestUserCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        User user = User.CreateGuest(integrationEvent.UserId);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
