using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Menu.Ingredients;

public sealed class IngredientNameValidator : AbstractValidator<string>
{
    public const int MinLength = 10;

    public const int MaxLength = 50;

    public IngredientNameValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Ingredient name cannot be empty.")
            .MinimumLength(MinLength).WithMessage($"Ingredient name minimum length is {MinLength}.")
            .MaximumLength(MaxLength).WithMessage($"Ingredient name maximum length is {MaxLength}.");
    }
}
