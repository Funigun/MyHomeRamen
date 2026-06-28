using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.UpdateIngredient;

internal static class Mappings
{
    internal static UpdateIngredientResponse ToResponse(this Ingredient ingredient)
        => new(ingredient.Id.Value);
}
