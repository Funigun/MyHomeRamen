using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Reservations.Tables;

public sealed class Table : AuditableEntity, IEntity<TableId>
{
    public TableId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public int TableNumber { get; private set; }

    public int MinNumberOfSeats { get; private set; }

    public int MaxNumberOfSeats { get; private set; }

    private Table()
    {
    }

    private Table(TableId id, Guid restaurantId)
    {
        Id = id;
        RestaurantId = restaurantId;
    }

    public static Table Create(TableId id, Guid restaurantId, int tableNumber, int minNumberOfSeats, int maxNumberOfSeats)
    {
        Table table = new(id, restaurantId)
        {
            TableNumber = tableNumber,
            MinNumberOfSeats = minNumberOfSeats,
            MaxNumberOfSeats = maxNumberOfSeats
        };

        TableValidator.Validate(table);

        return table;
    }
}
