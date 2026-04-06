using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
    public static async Task<IEnumerable<TEntity>> GetByIds<TEntity, TId>(this IQueryable<TEntity> query, IEnumerable<TId> keys, CancellationToken cancellationToken)
                  where TEntity : class, IEntity<TId>
                  where TId : IEntityId
    {
        return await query.Where(e => keys.Contains(e.Id))
                          .ToListAsync(cancellationToken);
    }

    public static async Task<TEntity> GetBySelectorAsync<TEntity, TId>(this IQueryable<TEntity> query, TId id, CancellationToken cancellationToken = default)
                  where TEntity : class, IEntity<TId>
                  where TId : IEntityId
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
        MemberExpression property = Expression.Property(parameter, nameof(IEntity<>.Id));
        ConstantExpression constant = Expression.Constant(id, typeof(TId));
        BinaryExpression body = Expression.Equal(property, constant);
        Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await query.FirstAsync(lambda, cancellationToken);
    }

    public static async Task<TEntity> GetBySelectorNotTrackedAsync<TEntity, TId>(this IQueryable<TEntity> query, TId id, CancellationToken cancellationToken = default)
              where TEntity : class, IEntity<TId>
              where TId : IEntityId
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
        MemberExpression property = Expression.Property(parameter, nameof(IEntity<>.Id));
        ConstantExpression constant = Expression.Constant(id, typeof(TId));
        BinaryExpression body = Expression.Equal(property, constant);
        Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await query.AsNoTracking().FirstAsync(lambda, cancellationToken);
    }

    public static async Task<bool> ExistsByIdAsync<TEntity, TId>(this IQueryable<TEntity> query, TId id, CancellationToken cancellationToken = default)
                  where TEntity : class, IEntity<TId>
                  where TId : IEntityId
    {
        ParameterExpression? parameter = Expression.Parameter(typeof(TEntity), "e");
        MemberExpression? property = Expression.Property(parameter, nameof(IEntity<>.Id));
        ConstantExpression? constant = Expression.Constant(id, typeof(TId));
        BinaryExpression? body = Expression.Equal(property, constant);
        Expression<Func<TEntity, bool>>? lambda = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

        return await query.AsNoTracking().AnyAsync(lambda, cancellationToken);
    }

    public static async Task<bool> IsNameUniqueAsync(this IQueryable<Domain.Menu.Products.Product> query, string name, CancellationToken cancellationToken = default)
    {
        return !await query.AnyAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public static async Task<bool> IsCategoryNameUniqueAsync(this IQueryable<Domain.Menu.Categories.Category> query, string name, CancellationToken cancellationToken = default)
    {
        return !await query.AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public static async Task<bool> IsIngredientNameUniqueAsync(this IQueryable<Domain.Menu.Ingredients.Ingredient> query, string name, CancellationToken cancellationToken = default)
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

    public static IQueryable<Domain.Menu.Ingredients.Ingredient> ForManage(
        this DbSet<Domain.Menu.Ingredients.Ingredient> ingredients,
        string? name,
        IEnumerable<Guid>? categoryIds)
    {
        IQueryable<Domain.Menu.Ingredients.Ingredient> query = ingredients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(i => i.Name.ToLower().Contains(name.ToLower()));
        }

        if (categoryIds is not null && categoryIds.Any())
        {
            List<Domain.Menu.Categories.CategoryId> ids = categoryIds.Select(id => (Domain.Menu.Categories.CategoryId)id).ToList();
            query = query.Where(i => i.Categories.Any(c => ids.Contains(c.Id)));
        }

        return query.OrderBy(i => i.Name);
    }

    public static async Task<bool> IsCategoryUsedByProductAsync(
        this IQueryable<Domain.Menu.Products.Product> query,
        Domain.Menu.Categories.CategoryId categoryId,
        CancellationToken cancellationToken = default)
    {
        return await query.AnyAsync(p => p.Categories.Any(c => c.Id == categoryId), cancellationToken);
    }

    public static async Task<bool> IsCategoryUsedByIngredientAsync(
        this IQueryable<Domain.Menu.Ingredients.Ingredient> query,
        Domain.Menu.Categories.CategoryId categoryId,
        CancellationToken cancellationToken = default)
    {
        return await query.AnyAsync(i => i.Categories.Any(c => c.Id == categoryId), cancellationToken);
    }

    public static async Task<bool> IsIngredientUsedAsBaseByProductAsync(
        this IQueryable<Domain.Menu.Products.Product> query,
        Domain.Menu.Ingredients.IngredientId ingredientId,
        CancellationToken cancellationToken = default)
    {
        return await query.AnyAsync(p => p.BaseIngredients.Any(i => i.Id == ingredientId), cancellationToken);
    }

    public static async Task<bool> IsIngredientUsedAsCustomByProductAsync(
        this IQueryable<Domain.Menu.Products.Product> query,
        Domain.Menu.Ingredients.IngredientId ingredientId,
        CancellationToken cancellationToken = default)
    {
        return await query.AnyAsync(p => p.CustomIngredients.Any(i => i.Id == ingredientId), cancellationToken);
    }

    public static Task<List<Domain.Menu.Categories.Category>> GetRemainingForResequencingAsync(this DbSet<Domain.Menu.Categories.Category> categories, Domain.Menu.Categories.CategoryType categoryType, Domain.Menu.Categories.CategoryId excludeId, CancellationToken cancellationToken = default)
    {
        return categories
            .Where(c => c.CategoryType == categoryType && c.Id != excludeId)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }
}
