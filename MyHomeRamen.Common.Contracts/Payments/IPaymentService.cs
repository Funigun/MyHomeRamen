namespace MyHomeRamen.Common.Contracts.Payments;

public interface IPaymentService
{
    Task<bool> ValidatePaymentSelectionAsync(Guid methodId, Guid channelId, CancellationToken ct);
}
