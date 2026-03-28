using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Payments.PaymentProviders;

namespace MyHomeRamen.Persistance.Payments.Converters;

public class PaymentProviderIdConverter : ValueConverter<PaymentProviderId, Guid>
{
    public PaymentProviderIdConverter() : base(id => id.Value, value => new PaymentProviderId(value)) { }
}
