using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Menu.RestaurantIngredientPrices;

namespace MyHomeRamen.Blazor.Features.Menu.RestaurantIngredientPrices.Models;

public sealed class IngredientPriceFormModelValidator : BaseValidator<IngredientPriceFormModel>
{
    public IngredientPriceFormModelValidator(IStringLocalizer<RestaurantIngredientPricesResources> text)
    {
        RuleFor(model => model.Price)
            .GreaterThanOrEqualTo(0).WithMessage(text["PriceMinimum"]);
    }
}
