using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.ShoppingCart.Roles;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.ShoppingCart.Features.Roles.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IRoleRepository
{
    public void Add(Role entity) => Roles.Add(entity);

    public void AddRange(IEnumerable<Role> entities) => Roles.AddRange(entities);

    public async Task<bool> Exists(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken)
        => await Roles.AnyAsync(predicate, cancellationToken);

    public void Delete(Role entity) => Roles.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Role, bool>> predicate, CancellationToken cancellationToken)
        => await Roles.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Role, bool>> filterPredicate, Dictionary<Expression<Func<Role, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Role>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Roles.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<Role, RoleId>.Count(CancellationToken cancellationToken)
        => await Roles.CountAsync(cancellationToken);

    IRoleQuery IRoleRepository.Query() => this;

    IRoleSpecification IRoleRepository.Specification() => this;
}
