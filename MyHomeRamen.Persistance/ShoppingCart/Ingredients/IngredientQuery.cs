using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IIngredientQuery
{
    public async Task<Ingredient?> ByIdAsync(IngredientId ingredientId, CancellationToken cancellationToken)
        => await Ingredients.AsNoTracking().FirstOrDefaultAsync(ingredient => ingredient.Id == ingredientId, cancellationToken);
}
