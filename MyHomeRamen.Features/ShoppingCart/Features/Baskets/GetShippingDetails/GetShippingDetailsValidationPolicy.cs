using FluentValidation;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetShippingDetails;

public sealed class GetShippingDetailsValidationPolicy : AbstractValidator<GetShippingDetailsQuery>
{
    public GetShippingDetailsValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x.BasketId)
            .MustBeAccessibleBasket(
                dbContext,
                query => query.UserId);
    }
}

