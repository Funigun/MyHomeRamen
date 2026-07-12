using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.PaymentGateways;
using MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IPaymentGatewayQuery
{
    public async Task<PaymentGateway?> ByIdAsync(PaymentGatewayId id, CancellationToken cancellationToken)
        => await PaymentGateways.AsNoTracking().FirstOrDefaultAsync(gateway => gateway.Id == id, cancellationToken);
}
