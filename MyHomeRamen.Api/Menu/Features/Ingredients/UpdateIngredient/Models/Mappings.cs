using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Models;

internal static class Mappings
{
    internal static UpdateIngredientResponse ToResponse(this Ingredient ingredient)
        => new(ingredient.Id.Value);
}
