using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Orders;

namespace MyHomeRamen.Persistance.Orders.Converters;

public class CustomerIdConverter : ValueConverter<CustomerId, Guid>
{
    public CustomerIdConverter() : base(id => id.Value, value => new CustomerId(value)) { }
}
