using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IPaymentChannelQuery
{
    public async Task<PaymentChannel?> ByIdAsync(PaymentChannelId id, CancellationToken cancellationToken)
        => await PaymentChannels.AsNoTracking().FirstOrDefaultAsync(channel => channel.Id == id, cancellationToken);
}
