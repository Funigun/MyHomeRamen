using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;

namespace MyHomeRamen.Blazor.Features.Payments.Common.Services;

public sealed class PaymentApiClient(HttpClient httpClient)
{
    public async Task<IEnumerable<GetAvailableMethodsResponse>> GetAvailableMethodsAsync(CancellationToken ct = default)
    {
        return await httpClient.GetFromJsonAsync<IEnumerable<GetAvailableMethodsResponse>>("/api/payments/methods/available", ct)
            ?? [];
    }
}
