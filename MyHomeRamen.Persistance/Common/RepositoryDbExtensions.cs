using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
    extension<TEntity>(IQueryable<TEntity> query)
        where TEntity : class
    {
        public IQueryable<TEntity> Paged(int pageNumber, int pageSize)
            => query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        public IQueryable<TEntity> OrderedBy<TKey>(Expression<Func<TEntity, TKey>> orderBy, string order)
            => order.ToLower() == "asc"  ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

        public async Task<bool> Exists(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken)
            => await query.AsNoTracking().AnyAsync(filter, cancellationToken);
    }
}
