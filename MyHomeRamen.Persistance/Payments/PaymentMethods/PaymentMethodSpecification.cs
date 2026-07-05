using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IPaymentMethodSpecification
{
    public async Task<PaymentMethod?> ByIdAsync(PaymentMethodId id, CancellationToken cancellationToken = default)
        => await PaymentMethodsQuery
            .Include(method => method.PaymentChannels)
            .Where(method => method.Id == id && method.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
}
