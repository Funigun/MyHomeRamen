using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class IngredientRepository : IIngredientQuery
{
    public async Task<Ingredient?> ByIdAsync(IngredientId ingredientId, CancellationToken cancellationToken)
        => await shoppingCartDbContext.Ingredients.AsNoTracking().FirstOrDefaultAsync(ingredient => ingredient.Id == ingredientId, cancellationToken);
}
