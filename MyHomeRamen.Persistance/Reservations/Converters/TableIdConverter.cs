using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Reservations;

namespace MyHomeRamen.Persistance.Reservations.Converters;

public class TableIdConverter : ValueConverter<TableId, Guid>
{
    public TableIdConverter() : base(id => id.Value, value => new TableId(value)) { }
}
