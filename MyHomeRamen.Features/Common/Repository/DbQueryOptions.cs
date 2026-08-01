using System.Linq.Expressions;

namespace MyHomeRamen.Features.Common.Repository;

public record DbQueryOptions<TEntity> 
        where TEntity : class
{
    public Expression<Func<TEntity, bool>>? Filter { get; init; }

    public Expression<Func<TEntity, object>>? OrderBy { get; init; }

    public string OrderDirection { get; init; } = "asc";
}

public record DbQueryOptions<TEntity, TProjection> : DbQueryOptions<TEntity> 
        where TEntity : class
        where TProjection : class
{
    public Expression<Func<TEntity, TProjection>>? Selector { get; init; }
}

public record PagedDbQueryOptions<TEntity> : DbQueryOptions<TEntity>
        where TEntity : class
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record PagedDbQueryOptions<TEntity, TProjection> : DbQueryOptions<TEntity, TProjection>
        where TEntity : class
        where TProjection : class
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
