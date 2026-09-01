using MyHomeRamen.Domain.Payments.PaymentGateways;

namespace MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;

public interface IPaymentGatewayLoader
{
    Task<PaymentGateway?> ByIdAsync(PaymentGatewayId id, CancellationToken cancellationToken);
}
