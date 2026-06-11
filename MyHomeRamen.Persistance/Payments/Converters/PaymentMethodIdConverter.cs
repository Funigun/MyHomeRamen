using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Payments.PaymentMethods;

namespace MyHomeRamen.Persistance.Payments.Converters;

public class PaymentMethodIdConverter : ValueConverter<PaymentMethodId, Guid>
{
    public PaymentMethodIdConverter() : base(id => id.Value, value => new PaymentMethodId(value))
    {
    }
}
