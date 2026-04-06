using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;

internal static class Mappings
{
    internal static GetIngredientByIdResponse ToResponse(this Ingredient ingredient)
        => new(
            ingredient.Id.Value,
            ingredient.Name,
            ingredient.Description,
            ingredient.Price,
            ingredient.Categories.Select(c => c.Id.Value));
}
