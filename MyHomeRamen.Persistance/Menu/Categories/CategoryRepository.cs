using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : ICategoryRepository
{
    public void Add(Category entity) => Categories.Add(entity);

    public Task<int> Count(Expression<Func<Category, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => predicate is null ? Categories.CountAsync(cancellationToken) : Categories.CountAsync(predicate, cancellationToken);

    public void Delete(Category entity) => Categories.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default)
    {
        int rows = await Categories.Where(predicate).ExecuteDeleteAsync(cancellationToken);

        await _cacheService.RemoveByTagsAsync(["Category"], cancellationToken);

        return rows;
    }

    public Task<int> ExecuteUpdate(Expression<Func<Category, bool>> filterPredicate, Dictionary<Expression<Func<Category, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken = default) => Categories.AnyAsync(predicate, cancellationToken);

    public Task<Category?> GetByIdOrDefault(CategoryId id, CancellationToken cancellationToken = default) => Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public ICategoryQuery Query() => this;

    public ICategorySpecification Specification() => this;
}

