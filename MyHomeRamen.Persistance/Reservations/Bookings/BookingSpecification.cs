using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Features.Reservations.Features.Bookings.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IBookingSpecification
{
    async Task<Booking> IBookingSpecification.ByIdAsync(BookingId bookingId, CancellationToken cancellationToken)
        => await Set<Booking>().FirstAsync(booking => booking.Id == bookingId, cancellationToken);
}
