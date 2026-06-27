using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
    extension<TEntity>(IQueryable<TEntity> query)
        where TEntity : class
    {
        public IQueryable<TEntity> Paged(int pageNumber, int pageSize)
            => query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        public async Task<bool> Exists(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
            => await query.AsNoTracking().AnyAsync(filter, cancellationToken);

        public IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>>? filter = null)
            => ApplyFilterAndOrder(query, filter);

        public IQueryable<TEntity> GetList<TKey>(
            Expression<Func<TEntity, TKey>> orderBy,
            Expression<Func<TEntity, bool>>? filter = null,
            bool ascending = true)
            => ApplyFilterAndOrder(query, filter, orderBy, ascending);

        public async Task<IReadOnlyList<TProjected>> GetList<TProjected>(
            Expression<Func<TEntity, TProjected>> selector,
            Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default)
            => await ToReadOnlyListAsync(ApplyFilterAndOrder(query, filter).Select(selector), cancellationToken);
        public async Task<IReadOnlyList<TProjected>> GetList<TKey, TProjected>(
            Expression<Func<TEntity, TProjected>> selector,
            Expression<Func<TEntity, TKey>> orderBy,
            Expression<Func<TEntity, bool>>? filter = null,
            bool ascending = true,
            CancellationToken cancellationToken = default)
            => await ToReadOnlyListAsync(ApplyFilterAndOrder(query, filter, orderBy, ascending).Select(selector), cancellationToken);

        public IQueryable<TEntity> GetListQuery(Expression<Func<TEntity, bool>>? filter = null)
            => ApplyFilterAndOrder(query.AsNoTracking(), filter);

        public IQueryable<TEntity> GetListQuery<TKey>(
            Expression<Func<TEntity, TKey>> orderBy,
            Expression<Func<TEntity, bool>>? filter = null,
            bool ascending = true)
            => ApplyFilterAndOrder(query.AsNoTracking(), filter, orderBy, ascending);

        public async Task<IReadOnlyList<TProjected>> GetListQuery<TProjected>(
            Expression<Func<TEntity, TProjected>> selector,
            Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default)
            => await ToReadOnlyListAsync(ApplyFilterAndOrder(query.AsNoTracking(), filter).Select(selector), cancellationToken);
        public async Task<IReadOnlyList<TProjected>> GetListQuery<TKey, TProjected>(
            Expression<Func<TEntity, TProjected>> selector,
            Expression<Func<TEntity, TKey>> orderBy,
            Expression<Func<TEntity, bool>>? filter = null,
            bool ascending = true,
            CancellationToken cancellationToken = default)
            => await ToReadOnlyListAsync(ApplyFilterAndOrder(query.AsNoTracking(), filter, orderBy, ascending).Select(selector), cancellationToken);
    }

    extension<TEntity, TId>(IQueryable<TEntity> query)
        where TEntity : class, IEntity<TId>
        where TId : IEntityId
    {
        public IQueryable<TEntity> GetById(TId id)
            => query.Where(BuildIdPredicate<TEntity, TId>(id));

        public async Task<TEntity> GetById(TId id, CancellationToken cancellationToken)
            => await query.FirstAsync(BuildIdPredicate<TEntity, TId>(id), cancellationToken);

        public async Task<TProjected> GetById<TProjected>
        (
            TId id,
            Expression<Func<TEntity, TProjected>> selector,
            CancellationToken cancellationToken = default
        )
        => await query.Where(BuildIdPredicate<TEntity, TId>(id))
                      .Select(selector)
                      .FirstAsync(cancellationToken);

        public IQueryable<TEntity> GetByIdQuery(TId id)
            => query.AsNoTracking()
                    .Where(BuildIdPredicate<TEntity, TId>(id));

        public async Task<TEntity> GetByIdQuery(TId id, CancellationToken cancellationToken)
            => await query.AsNoTracking().Where(BuildIdPredicate<TEntity, TId>(id)).FirstAsync(cancellationToken);

        public async Task<TProjected> GetByIdQuery<TProjected>
        (
            TId id,
            Expression<Func<TEntity, TProjected>> selector,
            CancellationToken cancellationToken = default
        )
        => await query.AsNoTracking()
                      .Where(BuildIdPredicate<TEntity, TId>(id))
                      .Select(selector)
                      .FirstAsync(cancellationToken);

        public async Task<IEnumerable<TEntity>> GetByIds(IEnumerable<TId> keys, CancellationToken cancellationToken)
            => await query.Where(e => keys.Contains(e.Id)).ToListAsync(cancellationToken);
    }

    private static IQueryable<TEntity> ApplyFilterAndOrder<TEntity>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>>? filter)
        where TEntity : class
    {
        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return query;
    }

    private static IQueryable<TEntity> ApplyFilterAndOrder<TEntity, TKey>(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>>? filter,
        Expression<Func<TEntity, TKey>> orderBy,
        bool ascending)
        where TEntity : class
    {
        if (filter is not null)
        {
            query = query.Where(filter);
        }

        return ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
    }

    private static async Task<IReadOnlyList<T>> ToReadOnlyListAsync<T>(
        IQueryable<T> query,
        CancellationToken cancellationToken)
        => await query.ToListAsync(cancellationToken);

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
