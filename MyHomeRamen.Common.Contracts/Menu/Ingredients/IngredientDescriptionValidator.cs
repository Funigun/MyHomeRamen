using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Menu.Ingredients;

public sealed class IngredientDescriptionValidator : AbstractValidator<string>
{
    public const int MinLength = 5;

    public const int MaxLength = 200;

    public IngredientDescriptionValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .MinimumLength(MinLength)
            .MaximumLength(MaxLength);
    }
}
