namespace MyHomeRamen.Features.Payments.ExternalApi;

public interface IPaymentService
{
    Task<bool> ValidatePaymentSelectionAsync(Guid methodId, Guid channelId, CancellationToken ct);
}
