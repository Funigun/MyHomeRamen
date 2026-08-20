using MyHomeRamen.Domain.ShoppingCart.Ingredients;

namespace MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;

public interface IIngredientLoader
{
    Task<Ingredient?> ByIdAsync(IngredientId ingredientId, CancellationToken cancellationToken);
}
