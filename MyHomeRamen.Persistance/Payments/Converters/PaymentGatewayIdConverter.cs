using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Payments.PaymentGateways;

namespace MyHomeRamen.Persistance.Payments.Converters;

public class PaymentGatewayIdConverter : ValueConverter<PaymentGatewayId, Guid>
{
    public PaymentGatewayIdConverter() : base(id => id.Value, value => new PaymentGatewayId(value))
    {
    }
}
