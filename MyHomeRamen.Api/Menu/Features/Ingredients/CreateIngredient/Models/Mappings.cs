using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;

internal static class Mappings
{
    public static Ingredient ToDomain(this CreateIngredientRequest request, List<Category> categories)
    {
        return Ingredient.Create(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.Price,
            new Collection<Category>(categories));
    }
}
