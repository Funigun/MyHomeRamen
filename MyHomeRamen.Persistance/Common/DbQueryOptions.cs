using System.Linq.Expressions;

namespace MyHomeRamen.Persistance.Common;

public sealed record DbQueryOptions<TEntity>(
    Expression<Func<TEntity, bool>>? Filter = null,
    Expression<Func<TEntity, object>>? OrderBy = null,
    string OrderDirection = "asc")
    where TEntity : class
{
    public static DbQueryOptions<TEntity> Empty { get; } = new();

    public static DbQueryOptions<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        => new(Filter: filter);

    public DbQueryOptions<TEntity> OrderByAsc(Expression<Func<TEntity, object>> orderBy)
        => this with { OrderBy = orderBy, OrderDirection = "asc" };

    public DbQueryOptions<TEntity> OrderByDesc(Expression<Func<TEntity, object>> orderBy)
        => this with { OrderBy = orderBy, OrderDirection = "desc" };
}
