using FluentValidation;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.ClearBasket;

public sealed class ClearBasketValidationPolicy : AbstractValidator<ClearBasketCommand>
{
    public ClearBasketValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x.BasketId)
            .MustBeAccessibleBasket(
                dbContext,
                command => command.UserId);
    }
}

