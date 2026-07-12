using MyHomeRamen.Domain.ShoppingCart.BasketItems;

namespace MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;

public interface IBasketItemSpecification
{
    Task<BasketItem?> ByIdAsync(BasketItemId basketItemId, CancellationToken cancellationToken);
}
