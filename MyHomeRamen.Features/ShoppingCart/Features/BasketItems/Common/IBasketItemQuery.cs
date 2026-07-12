using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;

public interface IBasketItemQuery
{
    Task<bool> ItemExistsAsync(UserId userId, BasketItemId basketItemId, BasketId basketId, CancellationToken cancellationToken);
}
