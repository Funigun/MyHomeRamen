using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

public interface IBasketQuery
{
    Task<Basket?> GetForUserAsync(UserId userId, CancellationToken cancellationToken);

    Task<Basket?> GetByIdForUserAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken);

    Task<Basket?> GetByIdForUserWithPaymentAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken);

    Task<Basket?> GetByIdForUserWithShippingAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken);

    Task<Basket?> GetCurrentBasketSummaryAsync(Guid userId, CancellationToken cancellationToken);
}
