using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.GetAvailableMethods;

namespace MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;

public interface IPaymentMethodQuery
{
    Task<List<GetAvailableMethodsResponse>> GetAvailableMethodsAsync(CancellationToken cancellationToken);

    Task<PaymentMethod?> GetByIdAsync(PaymentMethodId id, CancellationToken cancellationToken);
}
