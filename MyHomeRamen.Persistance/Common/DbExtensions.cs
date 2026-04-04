using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Persistance.Common;

public static class DbExtensions
{
    public static async Task<IEnumerable<TEntity>> GetByIds<TEntity, TId>(this IQueryable<TEntity> query, IEnumerable<TId> keys, CancellationToken cancellationToken)
                  where TEntity : class, IEntity<TId>
                  where TId : IEntityId
    {
        return await query.Where(e => keys.Contains(e.Id))
                          .ToListAsync(cancellationToken);
    }

    public static async Task<IEnumerable<TEntity>> GetBySelector<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken)
                  where TEntity : class, IEntity
    {
        return await query.Where(expression)
                          .ToListAsync(cancellationToken);
    }

    public static async Task<bool> ExistsByIdAsync<TEntity, TId>(
        this IQueryable<TEntity> query,
        TId id,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TId>
        where TId : IEntityId
    {
        ParameterExpression? parameter = Expression.Parameter(typeof(TEntity), "e");
        MemberExpression? property = Expression.Property(parameter, nameof(IEntity<TId>.Id));
        ConstantExpression? constant = Expression.Constant(id, typeof(TId));
        BinaryExpression? body = Expression.Equal(property, constant);
        Expression<Func<TEntity, bool>>? lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await query.AnyAsync(lambda, cancellationToken);
    }

    public static async Task<bool> IsNameUniqueAsync(
        this IQueryable<MyHomeRamen.Domain.Menu.Products.Product> query,
        string name,
        CancellationToken cancellationToken = default)
    {
        return !await query.AnyAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public static async Task<bool> IsCategoryNameUniqueAsync(this IQueryable<Domain.Menu.Categories.Category> query, string name, CancellationToken cancellationToken = default)
    {
        return !await query.AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public static async Task<bool> IsIngredientNameUniqueAsync(
        this IQueryable<Domain.Menu.Ingredients.Ingredient> query,
        string name,
        CancellationToken cancellationToken = default)
    {
        return !await query.AnyAsync(i => i.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public static async Task<int> GetNextSortOrderAsync(this IQueryable<Domain.Menu.Categories.Category> query, Domain.Menu.Categories.CategoryType categoryType, CancellationToken cancellationToken = default)
    {
        bool any = await query.AnyAsync(c => c.CategoryType == categoryType, cancellationToken);

        if (!any)
        {
            return Domain.Common.Category.CategoryConstants.MinSortOrder;
        }

        return await query.Where(c => c.CategoryType == categoryType)
                          .MaxAsync(c => c.SortOrder, cancellationToken) + 1;
    }

    public static IQueryable<Domain.Menu.Categories.Category> ForCategoryType(this DbSet<Domain.Menu.Categories.Category> categories, Domain.Menu.Categories.CategoryType categoryType)
    {
        return categories
            .AsNoTracking()
            .Where(c => c.CategoryType == categoryType)
            .OrderBy(c => c.SortOrder);
    }

    public static IQueryable<Domain.Menu.Ingredients.Ingredient> ForDropdown(this DbSet<Domain.Menu.Ingredients.Ingredient> ingredients)
    {
        return ingredients
            .AsNoTracking()
            .OrderBy(i => i.Name);
    }
}
