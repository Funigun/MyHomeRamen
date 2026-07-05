using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Reservations.Features.Bookings.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IBookingRepository
{
    public void Add(Booking entity) => Set<Booking>().Add(entity);

    public void AddRange(IEnumerable<Booking> entities) => Set<Booking>().AddRange(entities);

    public async Task<bool> Exists(Expression<Func<Booking, bool>> predicate, CancellationToken cancellationToken)
        => await Set<Booking>().AnyAsync(predicate, cancellationToken);

    public void Delete(Booking entity) => Set<Booking>().Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Booking, bool>> predicate, CancellationToken cancellationToken)
        => await Set<Booking>().Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Booking, bool>> filterPredicate, Dictionary<Expression<Func<Booking, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Booking>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Set<Booking>().Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<Booking, BookingId>.Count(CancellationToken cancellationToken)
        => await Set<Booking>().CountAsync(cancellationToken);

    IBookingQuery IBookingRepository.Query() => this;

    IBookingSpecification IBookingRepository.Specification() => this;
}
