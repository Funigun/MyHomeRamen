using Microsoft.AspNetCore.Identity;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Reservations.Tables;

namespace MyHomeRamen.Domain.Reservations.Bookings;

public sealed class Booking : AuditableEntity, IEntity<BookingId>
{
    private readonly List<Table> _tables = [];

    public BookingId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public BookingStatus Status { get; private set; }

    public IReadOnlyList<Table> Tables => _tables.ToList();

    private Booking()
    {
    }

    private Booking(BookingId id, Guid restaurantId, IEnumerable<Table> tables)
    {
        Id = id;
        RestaurantId = restaurantId;
        _tables.AddRange(tables);
    }

    public static Booking Create(BookingId id, Guid restaurantId, IEnumerable<Table> tables)
    {
        Booking booking = new(id, restaurantId, tables)
        {
            Status = BookingStatus.Created
        };

        BookingValidator.Validate(booking);

        return booking;
    }

    public void Confirm()
    {
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
    }

    public void MarkAsCompleted()
    {
        Status = BookingStatus.Paid;
    }
}
