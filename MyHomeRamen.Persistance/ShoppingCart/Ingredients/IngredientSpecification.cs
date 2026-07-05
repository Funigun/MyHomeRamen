using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IIngredientSpecification
{
    async Task<Ingredient?> IIngredientSpecification.ByIdAsync(IngredientId ingredientId, CancellationToken cancellationToken)
        => await Ingredients.FirstOrDefaultAsync(ingredient => ingredient.Id == ingredientId, cancellationToken);
}
