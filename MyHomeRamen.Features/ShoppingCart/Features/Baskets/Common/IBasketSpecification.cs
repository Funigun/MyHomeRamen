using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

public interface IBasketSpecification
{
    Task<Basket> GetForUserTrackedAsync(UserId userId, CancellationToken cancellationToken);

    Task<Basket> GetByIdForUserTrackedAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken);

    Task<Basket> GetByIdForUserWithPaymentTrackedAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken);

    Task<Basket> GetByIdForUserWithShippingTrackedAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken);
}
