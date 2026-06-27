using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetPaymentDetails;

public sealed class GetPaymentDetailsHandler(IShoppingCartDbContext dbContext) : IQueryHandler<GetPaymentDetailsQuery, PaymentDetailsResponse>
{
    public async Task<PaymentDetailsResponse> Handle(GetPaymentDetailsQuery query, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.ShoppingCarts
            .GetByIdForUserWithPayment(query.BasketId, query.UserId)
            .FirstAsync(cancellationToken);

        return new PaymentDetailsResponse(
            basket.PaymentDetails?.PaymentMethodId ?? string.Empty,
            basket.PaymentDetails?.PaymentChannelId ?? string.Empty
        );
    }
}
