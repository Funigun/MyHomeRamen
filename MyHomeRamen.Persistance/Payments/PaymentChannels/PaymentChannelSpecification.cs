using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentChannelRepository : IPaymentChannelSpecification
{
    async Task<PaymentChannel?> IPaymentChannelSpecification.ByIdAsync(PaymentChannelId id, CancellationToken cancellationToken)
        => await paymentsDbContext.PaymentChannels.AsNoTracking().FirstOrDefaultAsync(channel => channel.Id == id, cancellationToken);
}
