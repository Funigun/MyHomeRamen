using MyHomeRamen.Common.Contracts.Menu.Ingredients.DTOs;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

internal static class Mappings
{
    public static IngredientForManageDto ToResponse(this Ingredient ingredient)
    {
        return new(ingredient.Id.Value, ingredient.Name, ingredient.Description);
    }
}
