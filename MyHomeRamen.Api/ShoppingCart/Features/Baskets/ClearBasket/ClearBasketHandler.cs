using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.ClearBasket;

public sealed class ClearBasketHandler(IShoppingCartDbContext dbContext) : ICommandHandler<ClearBasketCommand>
{
    public async Task Handle(ClearBasketCommand command, CancellationToken cancellationToken)
    {
        Basket? basket = await dbContext.ShoppingCarts
            .GetByIdForUserTracked(command.BasketId, command.UserId)
            .SingleAsync(cancellationToken);

        basket.Clear();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
