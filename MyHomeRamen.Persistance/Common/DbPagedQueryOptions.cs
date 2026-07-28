using System.Linq.Expressions;
using MyHomeRamen.Features.Common.Endpoints.Models;

namespace MyHomeRamen.Persistance.Common;

public sealed record DbPagedQueryOptions<TEntity>(
    Expression<Func<TEntity, bool>>? Filter = null,
    Expression<Func<TEntity, object>>? OrderBy = null,
    string OrderDirection = "asc",
    int PageNumber = 1,
    int PageSize = 10)
    where TEntity : class
{
    public static DbPagedQueryOptions<TEntity> From(DbQueryOptions<TEntity> query, PageParameters page)
        => new(query.Filter, query.OrderBy, query.OrderDirection, page.PageNumber, page.PageSize);

    public static DbPagedQueryOptions<TEntity> From(
        DbQueryOptions<TEntity> query,
        PageParameters page,
        OrderParameters order,
        Expression<Func<TEntity, object>> orderBy)
        => new(query.Filter, orderBy, order.SortOrder, page.PageNumber, page.PageSize);
}
