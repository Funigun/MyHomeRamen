using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Orders;

namespace MyHomeRamen.Persistance.Orders.Converters;

public class PaymentIdConverter : ValueConverter<PaymentId, Guid>
{
    public PaymentIdConverter() : base(id => id.Value, value => new PaymentId(value)) { }
}
