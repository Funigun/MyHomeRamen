using MyHomeRamen.Domain.Orders.Ingredients;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Orders.Features.Ingredients.Common;

public interface IIngredientRepository : IRepository<Ingredient, IngredientId>
{
    IIngredientQuery Query();

    IIngredientLoader Load();
}
