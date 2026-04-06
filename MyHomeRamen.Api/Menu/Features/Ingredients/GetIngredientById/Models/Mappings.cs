using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;

internal static class Mappings
{
    public static GetIngredientByIdResponse ToResponse(this Ingredient ingredient)
    {
        return new(
            ingredient.Id.Value,
            ingredient.Name,
            ingredient.Description,
            ingredient.Price,
            ingredient.Categories.Select(c => new IngredientCategoryDto(c.Id.Value, c.Name)));
    }
}
