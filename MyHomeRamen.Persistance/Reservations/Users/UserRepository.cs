using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Reservations.Features.Users.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IUserRepository
{
    public void Add(User entity) => Set<User>().Add(entity);

    public void AddRange(IEnumerable<User> entities) => Set<User>().AddRange(entities);

    public async Task<bool> Exists(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken)
        => await Set<User>().AnyAsync(predicate, cancellationToken);

    public void Delete(User entity) => Set<User>().Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken)
        => await Set<User>().Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<User, bool>> filterPredicate, Dictionary<Expression<Func<User, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<User>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Set<User>().Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<User, UserId>.Count(CancellationToken cancellationToken)
        => await Set<User>().CountAsync(cancellationToken);

    IUserQuery IUserRepository.Query() => this;

    IUserSpecification IUserRepository.Specification() => this;
}
