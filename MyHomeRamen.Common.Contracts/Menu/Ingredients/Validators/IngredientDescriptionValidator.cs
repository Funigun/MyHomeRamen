using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Validators;

public sealed class IngredientDescriptionValidator : AbstractValidator<string>
{
    public const int MinLength = 5;

    public const int MaxLength = 200;

    public IngredientDescriptionValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Ingredient description cannot be empty.")
            .MinimumLength(MinLength).WithMessage($"Ingredient description minimum length is {MinLength}.")
            .MaximumLength(MaxLength).WithMessage($"Ingredient description maximum length is {MaxLength}.");
    }
}
