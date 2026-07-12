using FluentValidation;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.DeleteBasketItem;

public sealed class DeleteBasketItemValidationPolicy : AbstractValidator<DeleteBasketItemCommand>
{
    public DeleteBasketItemValidationPolicy(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    {
        RuleFor(x => x.BasketId)
            .NotEmpty()
            .MustAsync(async (basketId, ct) =>
            {
                UserId userId = new(currentUser.UserId);
                return await dbContext.Basket.Query().GetByIdForUserAsync(basketId, userId, ct) != null;
            })
            .WithMessage("Basket was not found or does not belong to the current user.");

        RuleFor(x => x.BasketItemId)
            .NotEmpty()
            .MustAsync(async (command, basketItemId, ct) =>
            {
                UserId userId = new(currentUser.UserId);
                Basket? basket = await dbContext.Basket.Specification().GetByIdForUserTrackedAsync(command.BasketId, userId, ct);
                return basket?.Items.Any(item => item.Id == basketItemId) ?? false;
            })
            .WithMessage("Basket item was not found in the specified basket.");
    }
}

