using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown.Models;

internal static class Mappings
{
    public static GetIngredientsForDropdownResponse ToResponse(this Ingredient ingredient)
    {
        return new GetIngredientsForDropdownResponse(ingredient.Id.Value, ingredient.Name);
    }
}
