using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.DeleteBasketItem;

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

