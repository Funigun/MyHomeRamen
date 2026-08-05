using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Persistance.Common;

public abstract class BaseRepository<TEntity, TId>(DbContext dbContext, ICacheService cacheService) : IRepository<TEntity, TId>
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
        => await dbContext.Set<TEntity>()
                          .Filtered(options.Filter)
                          .OrderedBy(options.OrderBy, options.OrderDirection)
                          .ToListAsync(cancellationToken);

    public async Task<TProjection> QueryFirst<TModel, TProjection>(IQueryable<TModel> query, DbQueryOptions<TModel, TProjection> options, CancellationToken cancellationToken)
           where TModel : class
           where TProjection : class
        => await query.AsNoTracking()
                      .Filtered(options.Filter)
                      .OrderedBy(options.OrderBy, options.OrderDirection)     
                      .Select(options.Selector!)
                      .FirstAsync(cancellationToken);

    public async Task<TProjection> QueryFirst<TModel, TProjection>(IQueryable<TModel> query, DbQueryOptions<TModel, TProjection> options, CachePolicy cachePolicy, CancellationToken cancellationToken)
           where TModel : class
           where TProjection : class
        => await cacheService.GetOrSetAsync
           (
               cachePolicy,
               async (cancellationToken) => await QueryFirst(query, options, cancellationToken),
               cancellationToken
           );

    public async Task<TProjection?> QueryFirstOrDefault<TModel, TProjection>(IQueryable<TModel> query, DbQueryOptions<TModel, TProjection> options, CancellationToken cancellationToken)
       where TModel : class
       where TProjection : class
    => await query.AsNoTracking()
                  .Filtered(options.Filter)
                  .OrderedBy(options.OrderBy, options.OrderDirection)
                  .Select(options.Selector!)
                  .FirstOrDefaultAsync(cancellationToken);

    public async Task<TProjection?> QueryFirstOrDefault<TModel, TProjection>(IQueryable<TModel> query, DbQueryOptions<TModel, TProjection> options, CachePolicy cachePolicy, CancellationToken cancellationToken)
           where TModel : class
           where TProjection : class
        => await cacheService.GetOrSetAsync
           (
               cachePolicy,
               async (cancellationToken) => await QueryFirstOrDefault(query, options, cancellationToken),
               cancellationToken
           );

    public async Task<IEnumerable<TProjection>> QueryList<TModel, TProjection>(IQueryable<TModel> query, DbQueryOptions<TModel, TProjection> options, CancellationToken cancellationToken)
           where TModel : class
           where TProjection : class
        => await query.AsNoTracking()
                      .Filtered(options.Filter)
                      .OrderedBy(options.OrderBy, options.OrderDirection)
                      .Select(options.Selector!)
                      .ToListAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> QueryList<TModel, TProjection>(IQueryable<TModel> query, DbQueryOptions<TModel, TProjection> options, CachePolicy cachePolicy, CancellationToken cancellationToken)
           where TModel : class
           where TProjection : class
           => await cacheService.GetOrSetAsync
              (
                  cachePolicy,
                  async (cancellationToken) => await QueryList(query, options, cancellationToken),                                                           
                  cancellationToken
              );

    public async Task<PagedResult<TProjection>> QueryPaged<TModel, TProjection>(IQueryable<TModel> query, PagedDbQueryOptions<TModel, TProjection> options, CancellationToken cancellationToken)
           where TModel : class
           where TProjection : class
    {
        query = query.AsNoTracking().Filtered(options.Filter);

        int totalCount = await query.CountAsync(cancellationToken);

        List<TProjection> items = await query.OrderedBy(options.OrderBy, options.OrderDirection)
                                             .Paged(options.PageNumber, options.PageSize)
                                             .Select(options.Selector!)
                                             .ToListAsync(cancellationToken);

        return new PagedResult<TProjection>(totalCount, items);
    }

    public async Task<PagedResult<TProjection>> QueryPaged<TModel, TProjection>(IQueryable<TModel> query, PagedDbQueryOptions<TModel, TProjection> options, CachePolicy cachePolicy, CancellationToken cancellationToken)
           where TModel : class
           where TProjection : class
           => await cacheService.GetOrSetAsync
              (
                  cachePolicy,
                  async (cancellationToken) => await QueryPaged(query, options, cancellationToken),
                  cancellationToken
              );
}
