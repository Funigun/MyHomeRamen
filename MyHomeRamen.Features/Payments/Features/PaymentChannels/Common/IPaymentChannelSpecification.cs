using MyHomeRamen.Domain.Payments.PaymentChannels;

namespace MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;

public interface IPaymentChannelSpecification
{
    Task<PaymentChannel?> ByIdAsync(PaymentChannelId id, CancellationToken cancellationToken);
}
