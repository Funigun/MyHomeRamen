using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Domain.Payments.PaymentGateways;
using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Persistance.Payments.Converters;

public class PaymentChannelIdConverter : ValueConverter<PaymentChannelId, Guid>
{
    public PaymentChannelIdConverter() : base(id => id.Value, value => new PaymentChannelId(value))
    {
    }
}
