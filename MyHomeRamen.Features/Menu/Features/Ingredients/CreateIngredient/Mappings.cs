using System.Collections.ObjectModel;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.CreateIngredient;

internal static class Mappings
{
    public static Ingredient ToDomain(this CreateIngredientRequest request, IEnumerable<Category> categories)
    {
        return Ingredient.Create(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.Price,
            new Collection<Category>(categories.ToList()));
    }
}
