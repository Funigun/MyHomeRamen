using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Payments.PaymentChannels;

namespace MyHomeRamen.Persistance.Payments.Converters;

public class PaymentChannelIdConverter : ValueConverter<PaymentChannelId, Guid>
{
    public PaymentChannelIdConverter() : base(id => id.Value, value => new PaymentChannelId(value))
    {
    }
}
