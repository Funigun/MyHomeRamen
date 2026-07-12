using MyHomeRamen.Domain.Payments.PaymentChannels;

namespace MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;

public interface IPaymentChannelQuery
{
    Task<PaymentChannel?> ByIdAsync(PaymentChannelId id, CancellationToken cancellationToken);
}
