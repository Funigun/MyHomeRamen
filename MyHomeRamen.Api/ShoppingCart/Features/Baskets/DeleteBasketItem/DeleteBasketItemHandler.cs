using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem;

public sealed class DeleteBasketItemHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    : ICommandHandler<DeleteBasketItemCommand>
{
    public async Task Handle(DeleteBasketItemCommand command, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        Basket basket = await dbContext.ShoppingCarts
            .GetByIdForUserTracked(command.BasketId, userId)
            .FirstAsync(cancellationToken);

        basket.RemoveItem(command.BasketItemId);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
