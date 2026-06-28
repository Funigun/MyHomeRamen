using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientById;

internal static class Mappings
{
    public static GetIngredientByIdResponse ToResponse(this Ingredient ingredient)
    {
        return new(
            ingredient.Id.Value,
            ingredient.Name,
            ingredient.Description,
            ingredient.Price,
            ingredient.Categories.Select(c => c.Id.Value));
    }
}
