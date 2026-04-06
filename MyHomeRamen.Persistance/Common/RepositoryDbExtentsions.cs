using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Persistance.Common;

/// <summary>
/// DB Extensions for common repository queries such as GetById, Exists, IsUnique, etc.
/// </summary>
public static partial class DbExtentsions
{
    public static IQueryable<TEntity> Paged<TEntity>(this IQueryable<TEntity> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
