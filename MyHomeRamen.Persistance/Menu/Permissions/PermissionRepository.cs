using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Menu.Permssions;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IPermissionRepository
{
    public void Add(Permission entity) 
        => Permissions.Add(entity);

    public void AddRange(IEnumerable<Permission> entities) 
        => Permissions.AddRange(entities);

    async Task<int> IRepository<Permission, PermissionId>.Count(CancellationToken cancellationToken)
        => await Permissions.CountAsync(cancellationToken);

    public void Delete(Permission entity) 
        => Permissions.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken)
        => await Permissions.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Permission, bool>> filterPredicate, Dictionary<Expression<Func<Permission, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Permission> settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Permissions.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken)
        => await Permissions.AsNoTracking().AnyAsync(predicate, cancellationToken);

    IPermissionQuery IPermissionRepository.Query() => this;

    IPermissionSpecification IPermissionRepository.Specification() => this;
}
