using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class IngredientRepository : IIngredientLoader
{
    async Task<Ingredient?> IIngredientLoader.ByIdAsync(IngredientId ingredientId, CancellationToken cancellationToken)
        => await shoppingCartDbContext.Ingredients.FirstOrDefaultAsync(ingredient => ingredient.Id == ingredientId, cancellationToken);
}
