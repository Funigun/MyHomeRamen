using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem;

public sealed class DeleteBasketItemValidationPolicy : AbstractValidator<DeleteBasketItemCommand>
{
    public DeleteBasketItemValidationPolicy(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    {
        RuleFor(x => x.BasketId)
            .NotEmpty()
            .MustAsync(async (basketId, ct) =>
            {
                UserId userId = new(currentUser.UserId);
                return await dbContext.ShoppingCarts.GetByIdForUser(basketId, userId).AnyAsync(ct);
            })
            .WithMessage("Basket was not found or does not belong to the current user.");

        RuleFor(x => x.BasketItemId)
            .NotEmpty()
            .MustAsync(async (command, basketItemId, ct) =>
                await dbContext.ShoppingCarts.ItemExistsQuery(new(currentUser.UserId), basketItemId, command.BasketId, ct))
            .WithMessage("Basket item was not found in the specified basket.");
    }
}
