using System.Linq.Expressions;
using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Features.Common.Repository;

public interface IRepository<TEntity, TId>
           where TEntity : IEntity<TId> 
           where TId : IEntityId 
{
    void Add(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    void Delete(TEntity entity);

    Task<int> ExecuteDelete(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task<int> ExecuteUpdate(Expression<Func<TEntity, bool>> filterPredicate, Dictionary<Expression<Func<TEntity, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken);

    Task<int> Count(CancellationToken cancellationToken);
}
