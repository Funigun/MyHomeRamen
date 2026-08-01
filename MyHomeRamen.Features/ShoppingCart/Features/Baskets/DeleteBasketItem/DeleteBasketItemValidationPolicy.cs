using FluentValidation;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.DeleteBasketItem;

public sealed class DeleteBasketItemValidationPolicy : AbstractValidator<DeleteBasketItemCommand>
{
    public DeleteBasketItemValidationPolicy(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    {
        RuleFor(x => x.BasketId)
            .MustBeAccessibleBasket(
                dbContext,
                _ => new UserId(currentUser.UserId));

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

