using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Users.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IUserRepository
{
    public void Add(User entity) => Users.Add(entity);

    public void AddRange(IEnumerable<User> entities) => Users.AddRange(entities);

    async Task<int> IRepository<User, UserId>.Count(CancellationToken cancellationToken)
        => await Users.CountAsync(cancellationToken);

    public void Delete(User entity) => Users.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken)
        => await Users.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<User, bool>> filterPredicate, Dictionary<Expression<Func<User, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<User>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Users.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    public async Task<bool> Exists(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken)
        => await Users.AnyAsync(predicate, cancellationToken);

    public async Task<bool> Exists(UserId userId, CancellationToken cancellationToken)
        => await Users.AsNoTracking().AnyAsync(user => user.Id == userId, cancellationToken);

    IUserQuery IUserRepository.Query() => this;

    IUserSpecification IUserRepository.Specification() => this;
}
