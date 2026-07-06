using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Reservations.Permissions;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Reservations.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IPermissionRepository
{
    public void Add(Permission entity) => Set<Permission>().Add(entity);

    public void AddRange(IEnumerable<Permission> entities) => Set<Permission>().AddRange(entities);

    public async Task<bool> Exists(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken)
        => await Set<Permission>().AnyAsync(predicate, cancellationToken);

    public void Delete(Permission entity) => Set<Permission>().Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Permission, bool>> predicate, CancellationToken cancellationToken)
        => await Set<Permission>().Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Permission, bool>> filterPredicate, Dictionary<Expression<Func<Permission, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Permission>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Set<Permission>().Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<Permission, PermissionId>.Count(CancellationToken cancellationToken)
        => await Set<Permission>().CountAsync(cancellationToken);

    IPermissionQuery IPermissionRepository.Query() => this;

    IPermissionSpecification IPermissionRepository.Specification() => this;
}
