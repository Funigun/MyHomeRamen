using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Reservations.Roles;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Reservations.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Reservations;

public sealed partial class ReservationsDbContext : IRoleRepository
{
    public void Add(Role entity) => Set<Role>().Add(entity);

    public void AddRange(IEnumerable<Role> entities) => Set<Role>().AddRange(entities);

    public async Task<bool> Exists(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken)
        => await Set<Role>().AnyAsync(predicate, cancellationToken);

    public void Delete(Role entity) => Set<Role>().Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken)
        => await Set<Role>().Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Role, bool>> filterPredicate, Dictionary<Expression<Func<Role, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Role>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Set<Role>().Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<Role, RoleId>.Count(CancellationToken cancellationToken)
        => await Set<Role>().CountAsync(cancellationToken);

    IRoleQuery IRoleRepository.Query() => this;

    IRoleSpecification IRoleRepository.Specification() => this;
}
