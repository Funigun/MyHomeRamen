using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetShippingDetails;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class BasketRepository : IBasketQuery
{
    public async Task<CurrentBasketDetailsDto?> GetCurrentBasketDetailsAsync(GetCurrentBasketDetailsQueryOptions options, CancellationToken cancellationToken)
        => await QueryFirstOrDefault(
            shoppingCartDbContext.ShoppingCarts
                .Include(b => b.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.BaseIngredients)
                .Include(b => b.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.CustomIngredients)
                .AsSplitQuery(),
            options,
            cancellationToken);

    public async Task<CurrentBasketSummaryDto?> GetCurrentBasketSummaryAsync(GetCurrentBasketSummaryQueryOptions options, CancellationToken cancellationToken)
        => await QueryFirstOrDefault(
            shoppingCartDbContext.ShoppingCarts
                .Include(b => b.Items)
                    .ThenInclude(i => i.Product)
                .AsSplitQuery(),
            options,
            cancellationToken);

    public async Task<PaymentDetailsDto?> GetPaymentDetailsAsync(GetPaymentDetailsQueryOptions options, CancellationToken cancellationToken)
        => await QueryFirstOrDefault(
            shoppingCartDbContext.ShoppingCarts
                .Include(b => b.PaymentDetails),
            options,
            cancellationToken);

    public async Task<ShippingDetailsDto?> GetShippingDetailsAsync(GetShippingDetailsQueryOptions options, CancellationToken cancellationToken)
        => await QueryFirstOrDefault(
            shoppingCartDbContext.ShoppingCarts
                .Include(b => b.ShippingDetails),
            options,
            cancellationToken);

    public async Task<Basket?> GetByIdForUserAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken)
        => await shoppingCartDbContext.ShoppingCarts
            .Where(b => b.Id == basketId && b.UserId == userId && b.Status == BasketStatus.Active)
            .FirstOrDefaultAsync(cancellationToken);
}
