using FluentValidation;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket.Models;
using MyHomeRamen.Common.Contracts.Basket;
using MyHomeRamen.Common.Contracts.Menu;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket.Policies;

public sealed class AddItemToBasketValidator : AbstractValidator<AddItemToBasketRequest>
{
    public AddItemToBasketValidator(IMenuService menuService)
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .SetValidator(new BasketItemQuantityValidator());

        RuleFor(x => x.BaseIngredients)
            .NotNull();

        RuleForEach(x => x.BaseIngredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(i => i.Id).NotEmpty();
                ingredient.RuleFor(i => i.Quantity).SetValidator(new BasketItemQuantityValidator());
            });

        RuleFor(x => x.CustomIngredients)
            .NotNull();

        RuleForEach(x => x.CustomIngredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(i => i.Id).NotEmpty();
                ingredient.RuleFor(i => i.Quantity).SetValidator(new BasketItemQuantityValidator());
            });

        RuleFor(x => x.Comments)
            .SetValidator(new BasketItemCommentValidator()!);

        RuleFor(x => x)
            .MustAsync(async (req, ct) =>
                await menuService.ValidateProductConfigurationAsync(
                    req.ProductId,
                    req.BaseIngredients.Select(i => i.Id).ToList(),
                    req.CustomIngredients.Select(i => i.Id).ToList(),
                    ct))
            .WithMessage("Product configuration is invalid: product does not exist or the selected ingredients are not valid for this product.");
    }
}
