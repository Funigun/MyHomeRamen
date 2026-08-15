using FluentValidation;
using MyHomeRamen.Blazor.Common.Models;
using MyHomeRamen.Blazor.Features.Menu.Ingredients.Components.Validators;

namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Components;

public sealed class IngredientValidator : BaseValidator<IngredientModel>
{
    public IngredientValidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new IngredientNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("Please select at least one category.");
    }
}
