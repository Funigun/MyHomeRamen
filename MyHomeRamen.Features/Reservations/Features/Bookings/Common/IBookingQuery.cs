using MyHomeRamen.Domain.Reservations.Bookings;

namespace MyHomeRamen.Features.Reservations.Features.Bookings.Common;

public interface IBookingQuery
{
    Task<Booking?> ByIdAsync(BookingId bookingId, CancellationToken cancellationToken);
}
