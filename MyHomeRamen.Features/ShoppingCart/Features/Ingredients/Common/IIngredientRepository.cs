using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;

public interface IIngredientRepository : IRepository<Ingredient, IngredientId>
{
    IIngredientQuery Query();

    IIngredientSpecification Specification();
}
