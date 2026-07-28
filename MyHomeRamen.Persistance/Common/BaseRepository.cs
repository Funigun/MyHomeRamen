using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Persistance.Common;

public abstract class BaseRepository<TEntity, TId>(DbContext dbContext) : IRepository<TEntity, TId>
                  where TEntity : class, IEntity<TId>
                  where TId : IEntityId, new()
{
    public void Add(TEntity entity) => dbContext.Set<TEntity>().Add(entity);

    public void AddRange(IEnumerable<TEntity> entities) => dbContext.Set<TEntity>().AddRange(entities);

    public async Task<int> Count(CancellationToken cancellationToken) => await dbContext.Set<TEntity>().CountAsync(cancellationToken);

    public void Delete(TEntity entity) => dbContext.Set<TEntity>().Remove(entity);

    public Task<int> ExecuteDelete(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) => dbContext.Set<TEntity>().Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<TEntity, bool>> filterPredicate, Dictionary<Expression<Func<TEntity, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        return await dbContext.Set<TEntity>().Where(filterPredicate)
                                             .ExecuteUpdateAsync(s =>
                                             {
                                                 foreach (KeyValuePair<Expression<Func<TEntity, object>>, Expression> setter in valuesToUpdate)
                                                 {
                                                     s = s.SetProperty(setter.Key, setter.Value);
                                                 }
                                             }
                                             , cancellationToken);
    }

    public Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) => dbContext.Set<TEntity>().AsNoTracking().AnyAsync(predicate, cancellationToken);

    public async Task<TEntity> First(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) => await dbContext.Set<TEntity>().FirstAsync(predicate, cancellationToken);

    public async Task<TEntity?> FirstOrDefault(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken) => await dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<IEnumerable<TEntity>> List(DbQueryOptions<TEntity> options, CancellationToken cancellationToken)
        => await Apply(dbContext.Set<TEntity>(), options).ToListAsync(cancellationToken);

    public async Task<TProjection> QueryFirst<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> selector, CancellationToken cancellationToken)
        => await dbContext.Set<TEntity>()
                          .AsNoTracking()
                          .Where(predicate)
                          .Select(selector)
                          .FirstAsync(cancellationToken);

    public async Task<TProjection?> QueryFirstOrDefault<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> selector, CancellationToken cancellationToken)
        => await dbContext.Set<TEntity>()
                          .AsNoTracking()
                          .Where(predicate)
                          .Select(selector)
                          .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> QueryList<TProjection>(DbQueryOptions<TEntity> options, Expression<Func<TEntity, TProjection>> selector, CancellationToken cancellationToken)
        => await Apply(dbContext.Set<TEntity>().AsNoTracking(), options)
                          .Select(selector)
                          .ToListAsync(cancellationToken);

    public async Task<PagedResult<TProjection>> QueryPaged<TProjection>(DbPagedQueryOptions<TEntity> options, Expression<Func<TEntity, TProjection>> selector, CancellationToken cancellationToken)
    {
        IQueryable<TEntity> query = Apply(dbContext.Set<TEntity>().AsNoTracking(), options);

        int totalCount = await query.CountAsync(cancellationToken);

        List<TProjection> items = await query.Paged(options.PageNumber, options.PageSize)
                                             .Select(selector)
                                             .ToListAsync(cancellationToken);

        return new PagedResult<TProjection>(totalCount, items);
    }

    private static IQueryable<TEntity> Apply(IQueryable<TEntity> source, DbQueryOptions<TEntity> options)
        => source.Filtered(options.Filter)
                 .OrderedBy(options.OrderBy, options.OrderDirection);

    private static IQueryable<TEntity> Apply(IQueryable<TEntity> source, DbPagedQueryOptions<TEntity> options)
        => source.Filtered(options.Filter)
                 .OrderedBy(options.OrderBy, options.OrderDirection);
}
