using FluentValidation;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

internal static class BasketValidatorExtensions
{
    public static IRuleBuilderOptions<T, int> MustBeValidBasketItemQuantity<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder.GreaterThanOrEqualTo(BasketConstants.MinQuantity)
                            .WithMessage($"Quantity must be greater than or equal to {BasketConstants.MinQuantity}.")

                          .LessThanOrEqualTo(BasketConstants.MaxQuantity)
                            .WithMessage($"Quantity must be less than or equal to {BasketConstants.MaxQuantity}.");
    }

    public static IRuleBuilderOptions<T, string?> MustBeValidBasketItemComment<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(BasketConstants.MaxCommentLength)
                            .WithMessage($"Comment must not exceed {BasketConstants.MaxCommentLength} characters.")
                            .When(x => x is not null);
    }

    public static IRuleBuilderOptions<T, BasketId> MustBeAccessibleBasket<T>(
        this IRuleBuilder<T, BasketId> ruleBuilder,
        IShoppingCartDbContext dbContext,
        Func<T, UserId> userIdSelector,
        Guid? restaurantId)
    {
        return ruleBuilder.NotEmpty()
                            .WithMessage("Basket ID must not be empty.")

                          .MustAsync(async (command, basketId, cancellationToken) =>
                                {
                                    if (!restaurantId.HasValue)
                                    {
                                        return false;
                                    }

                                    UserId userId = userIdSelector(command);
                                    Basket? basket = await dbContext.Basket.Query()
                                        .GetByIdForUserAsync(basketId, userId, cancellationToken);
                                    return basket?.RestaurantId == restaurantId;
                                })
                            .WithMessage("Basket was not found or does not belong to the current user and restaurant.");
    }

    public static IRuleBuilderOptions<T, TCommand> MustHaveAccessibleBasket<T, TCommand>(
        this IRuleBuilder<T, TCommand> ruleBuilder,
        IShoppingCartDbContext dbContext,
        Func<TCommand, BasketId> basketIdSelector,
        Func<TCommand, UserId> userIdSelector,
        Guid? restaurantId,
        string? propertyName = null)
    {
        IRuleBuilderOptions<T, TCommand> result = ruleBuilder.MustAsync(async (command, cancellationToken) =>
                           {
                               if (!restaurantId.HasValue)
                               {
                                   return false;
                               }

                               Basket? basket = await dbContext.Basket.Query()
                                   .GetByIdForUserAsync(basketIdSelector(command), userIdSelector(command), cancellationToken);
                               return basket?.RestaurantId == restaurantId;
                           })
                            .WithMessage("Basket was not found or does not belong to the current user and restaurant.");

        return propertyName is null
            ? result
            : result.OverridePropertyName(propertyName);
    }
}
