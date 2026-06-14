using FluentValidation;
using MyHomeRamen.Blazor.Common.Models;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Validators;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Models;

public sealed class ProductCustomizationValidator : BaseValidator<ProductCustomizationModel>
{
    public ProductCustomizationValidator()
    {
        RuleFor(x => x.Quantity)
            .SetValidator(new BasketItemQuantityValidator());

        RuleForEach(x => x.BaseIngredients)
            .SetValidator(new IngredientCustomizationValidator(validateOnlyWhenSelected: false));

        RuleForEach(x => x.CustomIngredients)
            .SetValidator(new IngredientCustomizationValidator(validateOnlyWhenSelected: true));

        RuleFor(x => x.Comments)
            .SetValidator(new BasketItemCommentValidator());
    }

    private sealed class IngredientCustomizationValidator : AbstractValidator<IngredientCustomizationModel>
    {
        public IngredientCustomizationValidator(bool validateOnlyWhenSelected)
        {
            When(
                x => !validateOnlyWhenSelected || x.IsSelected,
                () => RuleFor(x => x.Quantity).SetValidator(new BasketItemQuantityValidator()));
        }
    }
}
