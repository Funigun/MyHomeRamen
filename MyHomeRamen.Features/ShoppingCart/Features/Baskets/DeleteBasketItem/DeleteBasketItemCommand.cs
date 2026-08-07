using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.DeleteBasketItem;

public sealed record DeleteBasketItemCommand(BasketId BasketId, BasketItemId BasketItemId) : ICommand;

public sealed class DeleteBasketItemHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    : ICommandHandler<DeleteBasketItemCommand>
{
    public async Task Handle(DeleteBasketItemCommand command, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        Basket basket = await dbContext.Basket.Specification()
            .GetByIdForUserTrackedAsync(command.BasketId, userId, cancellationToken)
            ?? throw new InvalidOperationException("Basket was not found.");

        basket.RemoveItem(command.BasketItemId);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

