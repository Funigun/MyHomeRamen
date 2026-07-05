using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Reservations.Tables;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Reservations.Features.Tables.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : ITableRepository
{
    public void Add(Table entity) => Set<Table>().Add(entity);

    public void AddRange(IEnumerable<Table> entities) => Set<Table>().AddRange(entities);

    public async Task<bool> Exists(Expression<Func<Table, bool>> predicate, CancellationToken cancellationToken)
        => await Set<Table>().AnyAsync(predicate, cancellationToken);

    public void Delete(Table entity) => Set<Table>().Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Table, bool>> predicate, CancellationToken cancellationToken)
        => await Set<Table>().Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Table, bool>> filterPredicate, Dictionary<Expression<Func<Table, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Table>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Set<Table>().Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<Table, TableId>.Count(CancellationToken cancellationToken)
        => await Set<Table>().CountAsync(cancellationToken);

    ITableQuery ITableRepository.Query() => this;

    ITableSpecification ITableRepository.Specification() => this;
}
