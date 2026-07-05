using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;
using MyHomeRamen.Domain.Payments.PaymentMethods;

namespace MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;

public interface IPaymentMethodQuery
{
    Task<List<GetAvailableMethodsResponse>> GetAvailableMethodsAsync(CancellationToken cancellationToken);

    Task<PaymentMethod?> GetByIdAsync(PaymentMethodId id, CancellationToken cancellationToken);
}
