using FluentValidation;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;

public sealed record GetPaymentDetailsQuery(BasketId BasketId, UserId UserId) : IQuery<PaymentDetailsResponse>;

public sealed class GetPaymentDetailsAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetPaymentDetailsQuery>
{
    public async Task<bool> Authorize(GetPaymentDetailsQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(currentUser.CanCheckout());
    }
}

public sealed class GetPaymentDetailsValidationPolicy : AbstractValidator<GetPaymentDetailsQuery>
{
    public GetPaymentDetailsValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x.BasketId)
            .MustBeAccessibleBasket(
                dbContext,
                query => query.UserId);
    }
}

public sealed record GetPaymentDetailsQueryOptions(BasketId BasketId, UserId UserId)
    : DbQueryOptions<Basket, PaymentDetailsDto>
    (
        new()
        {
            Filter = basket => basket.Id == BasketId && basket.UserId == UserId && basket.Status == BasketStatus.Active,
            Selector = basket => new PaymentDetailsDto(basket.PaymentDetails.PaymentMethodId, basket.PaymentDetails.PaymentChannelId)
        }
    );

public sealed class GetPaymentDetailsHandler(IShoppingCartDbContext dbContext) : IQueryHandler<GetPaymentDetailsQuery, PaymentDetailsResponse>
{
    public async Task<PaymentDetailsResponse> Handle(GetPaymentDetailsQuery query, CancellationToken cancellationToken)
    {
        PaymentDetailsDto? paymentDetails = await dbContext.Basket.Query()
            .GetPaymentDetailsAsync(new GetPaymentDetailsQueryOptions(query.BasketId, query.UserId), cancellationToken);

        return paymentDetails is null
            ? throw new InvalidOperationException("Basket was not found.")
            : new PaymentDetailsResponse(paymentDetails.PaymentMethodId, paymentDetails.PaymentChannelId);
    }
}
