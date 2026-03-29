using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Menu.Ingredients;

public sealed class IngredientPriceValidator : AbstractValidator<decimal>
{
    public const decimal MinPrice = 0.0m;

    public const decimal MaxPrice = 50.0m;

    public IngredientPriceValidator()
    {
        RuleFor(x => x)
            .GreaterThanOrEqualTo(MinPrice)
            .LessThanOrEqualTo(MaxPrice);
    }
}
