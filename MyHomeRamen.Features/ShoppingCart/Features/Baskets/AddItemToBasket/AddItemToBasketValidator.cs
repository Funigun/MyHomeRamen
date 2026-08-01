using FluentValidation;
using MyHomeRamen.Common.Contracts.Menu;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed class AddItemToBasketValidator : AbstractValidator<AddItemToBasketCommand>
{
    public AddItemToBasketValidator(IMenuService menuService)
    {
        RuleFor(x => x.AddItemToBasketRequest.ProductId)
            .NotEmpty();

        RuleFor(x => x.AddItemToBasketRequest.Quantity)
            .MustBeValidBasketItemQuantity();

        RuleFor(x => x.AddItemToBasketRequest.BaseIngredients)
            .NotNull();

        RuleForEach(x => x.AddItemToBasketRequest.BaseIngredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(i => i.Id).NotEmpty();
                ingredient.RuleFor(i => i.Quantity).MustBeValidBasketItemQuantity();
            });

        RuleFor(x => x.AddItemToBasketRequest.CustomIngredients)
            .NotNull();

        RuleForEach(x => x.AddItemToBasketRequest.CustomIngredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(i => i.Id).NotEmpty();
                ingredient.RuleFor(i => i.Quantity).MustBeValidBasketItemQuantity();
            });

        RuleFor(x => x.AddItemToBasketRequest.Comments)
            .MustBeValidBasketItemComment();

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
                await menuService.ValidateProductConfigurationAsync(
                    cmd.AddItemToBasketRequest.ProductId,
                    cmd.AddItemToBasketRequest.BaseIngredients.Select(i => i.Id).ToList(),
                    cmd.AddItemToBasketRequest.CustomIngredients.Select(i => i.Id).ToList(),
                    ct))
            .WithMessage("Product configuration is invalid: product does not exist or the selected ingredients are not valid for this product.");
    }
}

