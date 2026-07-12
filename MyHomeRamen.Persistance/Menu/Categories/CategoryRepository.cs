using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : ICategoryRepository
{
    public void Add(Category entity) 
        => Categories.Add(entity);

    public void AddRange(IEnumerable<Category> entities) 
        => Categories.AddRange(entities);

    async Task<int> IRepository<Category, CategoryId>.Count(CancellationToken cancellationToken) 
        => await Categories.CountAsync(cancellationToken);

    public void Delete(Category entity) 
        => Categories.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken) 
        => await Categories.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Category, bool>> filterPredicate, Dictionary<Expression<Func<Category, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Category> settersBuilder = PrepareSettersBuilder<Category>(valuesToUpdate);
        return await Categories.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken) 
        => await Categories.AsNoTracking().AnyAsync(predicate, cancellationToken);

    ICategoryQuery ICategoryRepository.Query() => this;

    ICategorySpecification ICategoryRepository.Specification() => this;
}

