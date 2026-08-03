using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;

public sealed record GetPaymentDetailsQuery(BasketId BasketId, UserId UserId) : IQuery<PaymentDetailsResponse>;

public sealed class GetPaymentDetailsHandler(IShoppingCartDbContext dbContext) : IQueryHandler<GetPaymentDetailsQuery, PaymentDetailsResponse>
{
    public async Task<PaymentDetailsResponse> Handle(GetPaymentDetailsQuery query, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.Basket.Query()
            .GetByIdForUserWithPaymentAsync(query.BasketId, query.UserId, cancellationToken)
            ?? throw new InvalidOperationException("Basket was not found.");

        return new PaymentDetailsResponse(
            basket.PaymentDetails?.PaymentMethodId ?? string.Empty,
            basket.PaymentDetails?.PaymentChannelId ?? string.Empty
        );
    }
}

