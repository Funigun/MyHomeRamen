using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Common.Contracts.Payments;
using MyHomeRamen.Domain.Payments.Database;
using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Persistance.Payments.Extensions;

namespace MyHomeRamen.Api.Payments.Services;

internal sealed class PaymentService(IPaymentsDbContext dbContext) : IPaymentService
{
    public async Task<bool> ValidatePaymentSelectionAsync(Guid methodId, Guid channelId, CancellationToken ct)
    {
        PaymentMethod? method = await dbContext.PaymentMethods.GetById(new(methodId))
                                                              .FirstOrDefaultAsync(ct);

        return method is not null && method.HasActiveChannel(new(channelId));
    }
}
