using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Features.Common.Repository;

public interface ICacheableRepository<TEntity, TId> : IRepository<TEntity, TId>
           where TEntity : IEntity<TId>
           where TId : IEntityId
{
}
