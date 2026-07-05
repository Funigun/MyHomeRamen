using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.ClearBasket;

public sealed class ClearBasketHandler(IShoppingCartDbContext dbContext) : ICommandHandler<ClearBasketCommand>
{
    public async Task Handle(ClearBasketCommand command, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.Basket.Specification()
            .GetByIdForUserTrackedAsync(command.BasketId, command.UserId, cancellationToken);

        basket.Clear();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

