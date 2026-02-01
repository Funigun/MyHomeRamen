using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Payments;

namespace MyHomeRamen.Persistance.Payments.Converters;

public class OrderIdConverter : ValueConverter<OrderId, Guid>
{
    public OrderIdConverter() : base(id => id.Value, value => new OrderId(value)) { }
}
