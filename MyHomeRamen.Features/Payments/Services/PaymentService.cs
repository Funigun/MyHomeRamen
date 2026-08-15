using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Features.Payments.ExternalApi;
using MyHomeRamen.Features.Payments.Features.Abstractions;

namespace MyHomeRamen.Features.Payments.Services;

public sealed class PaymentService(IPaymentsDbContext dbContext) : IPaymentService
{
    public async Task<bool> ValidatePaymentSelectionAsync(Guid methodId, Guid channelId, CancellationToken ct)
    {
        PaymentMethod? method = await dbContext.PaymentMethod.Query().GetByIdAsync(new(methodId), ct);

        if (method == null) { return false; }

        bool selectedMethodWithoutChannel = !method.HasChannels() && channelId == Guid.Empty;
        bool selectedMethodWithChannel = method.HasChannels() && method.HasActiveChannel(new(channelId));

        return selectedMethodWithoutChannel || selectedMethodWithChannel;
    }
}
