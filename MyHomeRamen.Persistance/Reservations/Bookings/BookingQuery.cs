using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Features.Reservations.Features.Bookings.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IBookingQuery
{
    public async Task<Booking?> ByIdAsync(BookingId bookingId, CancellationToken cancellationToken = default)
        => await Set<Booking>().AsNoTracking().FirstOrDefaultAsync(booking => booking.Id == bookingId, cancellationToken);
}
