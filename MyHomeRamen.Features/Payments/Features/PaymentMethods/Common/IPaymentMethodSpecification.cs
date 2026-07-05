using MyHomeRamen.Domain.Payments.PaymentMethods;

namespace MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;

public interface IPaymentMethodSpecification
{
    Task<PaymentMethod?> ByIdAsync(PaymentMethodId id, CancellationToken cancellationToken = default);
}
