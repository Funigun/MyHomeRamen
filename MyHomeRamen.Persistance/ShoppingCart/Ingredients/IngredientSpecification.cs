using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class IngredientRepository : IIngredientSpecification
{
    async Task<Ingredient?> IIngredientSpecification.ByIdAsync(IngredientId ingredientId, CancellationToken cancellationToken)
        => await shoppingCartDbContext.Ingredients.FirstOrDefaultAsync(ingredient => ingredient.Id == ingredientId, cancellationToken);
}
