using FluentValidation;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;

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

