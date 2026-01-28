using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Reservations;

public sealed class Table : AuditableEntity, IEntity<TableId>
{
    public TableId Id { get; private set; }

    public int TableNumber { get; private set; }

    public int MinNumberOfSeats { get; private set; }

    public int MaxNumberOfSeats { get; private set; }

    private Table()
    {
    }

    private Table(TableId id)
    {
        Id = id;
    }

    public static Table Create(TableId id, int tableNumber, int minNumberOfSeats, int maxNumberOfSeats)
    {
        return new Table(id)
        {
            TableNumber = tableNumber,
            MinNumberOfSeats = minNumberOfSeats,
            MaxNumberOfSeats = maxNumberOfSeats
        };
    }
}
