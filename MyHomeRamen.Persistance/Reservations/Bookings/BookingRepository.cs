using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Reservations.Features.Bookings.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Reservations;

public sealed partial class BookingRepository(ReservationsDbContext reservationsDbContext, ICacheService cacheService)
    : BaseRepository<Booking, BookingId>(reservationsDbContext, cacheService), IBookingRepository
{
    public IBookingQuery Query() => this;

    public IBookingLoader Load() => this;
}