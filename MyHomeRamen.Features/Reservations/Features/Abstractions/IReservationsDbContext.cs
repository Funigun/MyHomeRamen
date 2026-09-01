using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Reservations.Features.Bookings.Common;
using MyHomeRamen.Features.Reservations.Features.Tables.Common;

namespace MyHomeRamen.Features.Reservations.Features.Abstractions;

public interface IReservationsDbContext : IUnitOfWork
{
    IBookingRepository Booking { get; }

    ITableRepository Table { get; }
}
