using System.Linq.Expressions;
using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Features.Common.Repository;

public interface IRepository<TEntity, TId>
           where TEntity : IEntity<TId> 
           where TId : IEntityId 
{
    void Add(TEntity entity);

    Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    void Delete(TEntity entity);

    Task<int> ExecuteDelete(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<int> ExecuteUpdate(Expression<Func<TEntity, bool>> filterPredicate, Dictionary<Expression<Func<TEntity, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken = default);

    Task<int> Count(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

