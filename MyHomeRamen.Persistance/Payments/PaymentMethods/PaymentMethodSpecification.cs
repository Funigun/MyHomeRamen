using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentMethodRepository : IPaymentMethodSpecification
{
    public async Task<PaymentMethod?> ByIdAsync(PaymentMethodId id, CancellationToken cancellationToken)
        => await paymentsDbContext.PaymentMethods.AsNoTracking()
            .Include(method => method.PaymentChannels)
            .Where(method => method.Id == id && method.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
}
