using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Features.Common.Repository;

public static partial class DbExtensions
{
    extension<TEntity>(IQueryable<TEntity> query)
        where TEntity : class
    {
        public IQueryable<TEntity> Paged(int pageNumber, int pageSize)
            => query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        public IQueryable<TEntity> Sorted<TKey>(Expression<Func<TEntity, TKey>> keySelector, string sortOrder)
            => sortOrder.ToLower() == "asc" ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector);

        public async Task<bool> Exists(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
            => await query.AsNoTracking().AnyAsync(filter, cancellationToken);
    }

    extension<TEntity, TId>(IQueryable<TEntity> query)
        where TEntity : class, IEntity<TId>
        where TId : IEntityId
    {
        public async Task<TEntity> GetById(TId id, CancellationToken cancellationToken)
            => await query.FirstAsync(BuildIdPredicate<TEntity, TId>(id), cancellationToken);


        public async Task<TEntity> GetByIdQuery(TId id, CancellationToken cancellationToken)
            => await query.AsNoTracking().Where(BuildIdPredicate<TEntity, TId>(id)).FirstAsync(cancellationToken);

        public async Task<IEnumerable<TEntity>> GetByIds(IEnumerable<TId> keys, CancellationToken cancellationToken)
            => await query.Where(e => keys.Contains(e.Id)).ToListAsync(cancellationToken);
    }


    private static Expression<Func<TEntity, bool>> BuildIdPredicate<TEntity, TId>(TId id)
        where TEntity : class, IEntity<TId>
        where TId : IEntityId
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
        MemberExpression property = Expression.Property(parameter, nameof(IEntity<>.Id));
        ConstantExpression constant = Expression.Constant(id, typeof(TId));
        BinaryExpression body = Expression.Equal(property, constant);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
}

