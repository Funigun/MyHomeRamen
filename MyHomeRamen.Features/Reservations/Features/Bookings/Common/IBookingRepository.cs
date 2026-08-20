using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Reservations.Features.Bookings.Common;

public interface IBookingRepository : IRepository<Booking, BookingId>
{
    IBookingQuery Query();

    IBookingLoader Load();
}
