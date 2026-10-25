using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Menu.Ingredients;

namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Models;

public sealed class IngredientFormModelValidator : BaseValidator<IngredientFormModel>
{
    public IngredientFormModelValidator(IStringLocalizer<IngredientsResources> text)
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage(text["NameRequired"])
            .MinimumLength(5).WithMessage(text["NameMinLength"])
            .MaximumLength(50).WithMessage(text["NameMaxLength"]);

        RuleFor(model => model.Description)
            .NotEmpty().WithMessage(text["DescriptionRequired"])
            .MinimumLength(5).WithMessage(text["DescriptionMinLength"])
            .MaximumLength(200).WithMessage(text["DescriptionMaxLength"]);

        RuleFor(model => model.Price)
            .InclusiveBetween(0m, 50m).WithMessage(text["PriceRange"]);

        RuleFor(model => model.CategoryIds)
            .NotEmpty().WithMessage(text["CategoryRequired"]);
    }
}
