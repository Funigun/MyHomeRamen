using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Payments;

namespace MyHomeRamen.Persistance.Payments.Converters;

public class PaymentIdConverter : ValueConverter<PaymentId, Guid>
{
    public PaymentIdConverter() : base(id => id.Value, value => new PaymentId(value)) { }
}
