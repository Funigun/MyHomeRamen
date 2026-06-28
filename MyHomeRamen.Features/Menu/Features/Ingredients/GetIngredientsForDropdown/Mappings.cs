using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForDropdown;

internal static class Mappings
{
    public static GetIngredientsForDropdownResponse ToResponse(this Ingredient ingredient)
    {
        return new GetIngredientsForDropdownResponse(ingredient.Id.Value, ingredient.Name);
    }
}
