using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IBasketQuery
{
    private IQueryable<Basket> BasketQuery => ShoppingCarts.AsNoTracking();

    public async Task<Basket?> GetForUserAsync(UserId userId, CancellationToken cancellationToken)
        => await BasketQuery
            .Where(b => b.User.Id == userId && b.Status == BasketStatus.Active)
            .Include(b => b.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.BaseIngredients)
            .Include(b => b.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.CustomIngredients)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Basket?> GetByIdForUserAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken)
        => await BasketQuery
            .Where(b => b.Id == basketId && b.User.Id == userId && b.Status == BasketStatus.Active)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Basket?> GetByIdForUserWithPaymentAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken)
        => await BasketQuery
            .Where(b => b.Id == basketId && b.User.Id == userId && b.Status == BasketStatus.Active)
            .Include(b => b.PaymentDetails)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Basket?> GetByIdForUserWithShippingAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken)
        => await BasketQuery
            .Where(b => b.Id == basketId && b.User.Id == userId && b.Status == BasketStatus.Active)
            .Include(b => b.ShippingDetails)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Basket?> GetCurrentBasketSummaryAsync(Guid userId, CancellationToken cancellationToken)
        => await BasketQuery
            .Where(b => b.User.Id == (UserId)userId && b.Status == BasketStatus.Active)
            .Include(b => b.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(cancellationToken);
}
