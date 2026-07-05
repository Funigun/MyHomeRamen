using MyHomeRamen.Domain.Payments.PaymentGateways;

namespace MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;

public interface IPaymentGatewaySpecification
{
    Task<PaymentGateway?> ByIdAsync(PaymentGatewayId id, CancellationToken cancellationToken = default);
}
