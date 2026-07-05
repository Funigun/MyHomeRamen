using MyHomeRamen.Common.Contracts.Messaging;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Worker.Common;

namespace MyHomeRamen.Worker.MessagesHandler.ShoppingCart;

internal sealed class ShoppingCartGuestRegisteredHandler(IShoppingCartDbContext dbContext) : IIntegrationEventHandler<GuestUserCreatedIntegrationEvent>
{
    public async Task HandleAsync(GuestUserCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        User user = User.CreateGuest(integrationEvent.GuestId);
        Basket basket = Basket.Create(new BasketId(Guid.CreateVersion7()), user);

        dbContext.User.Add(user);
        dbContext.Basket.Add(basket);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
