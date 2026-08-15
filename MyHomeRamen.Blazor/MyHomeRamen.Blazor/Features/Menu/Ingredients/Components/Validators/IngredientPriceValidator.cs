using FluentValidation;

namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Components.Validators;

public sealed class IngredientPriceValidator : AbstractValidator<decimal>
{
    public const decimal MinPrice = 0.0m;

    public const decimal MaxPrice = 50.0m;

    public IngredientPriceValidator()
    {
        RuleFor(x => x)
            .LessThanOrEqualTo(MaxPrice).WithMessage($"Ingredient price cannot be greater than or equal to {MaxPrice}.")
            .GreaterThanOrEqualTo(MinPrice).WithMessage($"Ingredient price cannot be less than or equal to {MinPrice}");
    }
}
