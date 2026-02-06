using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Payments.PaymentGroups;

namespace MyHomeRamen.Persistance.Payments.Converters;

public class PaymentGroupIdConverter : ValueConverter<PaymentGroupId, Guid>
{
    public PaymentGroupIdConverter() : base(id => id.Value, value => new PaymentGroupId(value)) { }
}
