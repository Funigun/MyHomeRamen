using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class IngredientRepository : IIngredientSpecification
{
    async Task<Ingredient> IIngredientSpecification.ById(IngredientId ingredientId, CancellationToken cancellationToken)
        => await menuDbContext.Ingredients.Include(i => i.Categories)
                                          .AsSplitQuery()
                                          .FirstAsync(i => i.Id == ingredientId, cancellationToken);

    async Task<IEnumerable<Ingredient>> IIngredientSpecification.ByIds(IEnumerable<IngredientId> ingredientIds, CancellationToken cancellationToken)
        => await List(new DbQueryOptions<Ingredient>() { Filter = i => ingredientIds.Contains(i.Id) }, cancellationToken);
}
