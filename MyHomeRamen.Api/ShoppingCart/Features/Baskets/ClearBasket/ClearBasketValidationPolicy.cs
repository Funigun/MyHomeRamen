using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.ClearBasket;

public sealed class ClearBasketValidationPolicy : AbstractValidator<ClearBasketCommand>
{
    public ClearBasketValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                bool basketExists = await dbContext.ShoppingCarts
                    .GetByIdForUser(command.BasketId, command.UserId)
                    .AnyAsync(ct);
                return basketExists;
            })
            .WithMessage("Basket not found or not accessible");
    }
}
