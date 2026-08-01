using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IBasketSpecification
{
    private IQueryable<Basket> TrackedBasketQuery => ShoppingCarts;

    public async Task<Basket> GetForUserTrackedAsync(UserId userId, CancellationToken cancellationToken)
        => await TrackedBasketQuery
            .Where(b => b.User.Id == userId && b.Status == BasketStatus.Active)
            .Include(b => b.Items)
            .FirstAsync(cancellationToken);

    public async Task<Basket> GetByIdForUserTrackedAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken)
        => await TrackedBasketQuery
            .Where(b => b.Id == basketId && b.User.Id == userId && b.Status == BasketStatus.Active)
            .Include(b => b.Items)
            .FirstAsync(cancellationToken);

    public async Task<Basket> GetByIdForUserWithPaymentTrackedAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken)
        => await TrackedBasketQuery
            .Where(b => b.Id == basketId && b.User.Id == userId && b.Status == BasketStatus.Active)
            .Include(b => b.PaymentDetails)
            .FirstAsync(cancellationToken);

    public async Task<Basket> GetByIdForUserWithShippingTrackedAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken)
        => await TrackedBasketQuery
            .Where(b => b.Id == basketId && b.User.Id == userId && b.Status == BasketStatus.Active)
            .Include(b => b.ShippingDetails)
            .FirstAsync(cancellationToken);
}
