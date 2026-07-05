using FluentValidation;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.ClearBasket;

public sealed class ClearBasketValidationPolicy : AbstractValidator<ClearBasketCommand>
{
    public ClearBasketValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                bool basketExists = await dbContext.Basket.Query()
                    .GetByIdForUserAsync(command.BasketId, command.UserId, ct) != null;
                return basketExists;
            })
            .WithMessage("Basket not found or not accessible");
    }
}

