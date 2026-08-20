using MyHomeRamen.Domain.Payments.PaymentMethods;

namespace MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;

public interface IPaymentMethodLoader
{
    Task<PaymentMethod?> ByIdAsync(PaymentMethodId id, CancellationToken cancellationToken);
}
