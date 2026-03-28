using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Reservations.Bookings;

namespace MyHomeRamen.Persistance.Reservations.Converters;

public class BookingIdConverter : ValueConverter<BookingId, Guid>
{
    public BookingIdConverter() : base(id => id.Value, value => new BookingId(value)) { }
}
