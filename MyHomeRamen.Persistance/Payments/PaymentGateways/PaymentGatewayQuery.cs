using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.PaymentGateways;
using MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentGatewayRepository : IPaymentGatewayQuery
{
    public async Task<PaymentGateway?> ByIdAsync(PaymentGatewayId id, CancellationToken cancellationToken)
        => await paymentsDbContext.PaymentGateways.AsNoTracking().FirstOrDefaultAsync(gateway => gateway.Id == id, cancellationToken);
}
