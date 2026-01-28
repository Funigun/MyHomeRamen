using Microsoft.AspNetCore.Identity;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Reservations;

public sealed class Booking : AuditableEntity, IEntity<BookingId>
{
    private readonly List<Table> _tables = [];

    public BookingId Id { get; private set; }

    public BookingStatus Status { get; private set; }

    public IReadOnlyList<Table> Tables => _tables.ToList();

    private Booking()
    {
    }

    private Booking(BookingId id, IEnumerable<Table> tables)
    {
        Id = id;
        _tables.AddRange(tables);
    }

    public static Booking Create(BookingId id, IEnumerable<Table> tables)
    {
        return new Booking(id, tables)
        {
            Status = BookingStatus.Created
        };
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
