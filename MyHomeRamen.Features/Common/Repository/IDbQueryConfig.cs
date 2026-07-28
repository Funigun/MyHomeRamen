using System.Linq.Expressions;
using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Features.Common.Repository;

public interface IDbQueryConfig<TEntity>
{
    Expression<Func<TEntity, bool>>? GetFilter();

    Expression<Func<TEntity, object>>? GetOrder();
}

public interface IProjectedDbQueryConfig<TEntity, TProjection> : IDbQueryConfig<TEntity>
{
    Expression<Func<TEntity, TProjection>> GetProjection();
}

public interface IPagedDbQueryConfig<TEntity> : IDbQueryConfig<TEntity>
{
    
}
