using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IIngredientRepository
{
    public void Add(Ingredient entity) 
        => Ingredients.Add(entity);

    public void AddRange(IEnumerable<Ingredient> entities) 
        => Ingredients.AddRange(entities);
      
    async Task<int> IRepository<Ingredient, IngredientId>.Count(CancellationToken cancellationToken) 
        => await Ingredients.CountAsync(cancellationToken);

    public void Delete(Ingredient entity) 
        => Ingredients.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Ingredient, bool>> predicate, CancellationToken cancellationToken)
        => await Ingredients.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Ingredient, bool>> filterPredicate, Dictionary<Expression<Func<Ingredient, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Ingredient>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Ingredients.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<Ingredient, bool>> predicate, CancellationToken cancellationToken)
        => await Ingredients.AsNoTracking().AnyAsync(predicate, cancellationToken);

    IIngredientQuery IIngredientRepository.Query() => this;

    IIngredientSpecification IIngredientRepository.Specification() => this;
}
