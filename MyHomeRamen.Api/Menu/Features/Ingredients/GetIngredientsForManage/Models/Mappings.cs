using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage.Models;

internal static class Mappings
{
    public static IngredientDto ToResponse(this Ingredient ingredient)
    {
        return new(ingredient.Id.Value, ingredient.Name, ingredient.Description);
    }
}
