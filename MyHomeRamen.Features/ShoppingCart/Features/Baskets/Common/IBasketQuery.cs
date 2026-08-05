using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetShippingDetails;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

public interface IBasketQuery
{
    Task<CurrentBasketDetailsDto?> GetCurrentBasketDetailsAsync(GetCurrentBasketDetailsQueryOptions options, CancellationToken cancellationToken);

    Task<CurrentBasketSummaryDto?> GetCurrentBasketSummaryAsync(GetCurrentBasketSummaryQueryOptions options, CancellationToken cancellationToken);

    Task<PaymentDetailsDto?> GetPaymentDetailsAsync(GetPaymentDetailsQueryOptions options, CancellationToken cancellationToken);

    Task<ShippingDetailsDto?> GetShippingDetailsAsync(GetShippingDetailsQueryOptions options, CancellationToken cancellationToken);

    Task<Basket?> GetByIdForUserAsync(BasketId basketId, UserId userId, CancellationToken cancellationToken);
}
