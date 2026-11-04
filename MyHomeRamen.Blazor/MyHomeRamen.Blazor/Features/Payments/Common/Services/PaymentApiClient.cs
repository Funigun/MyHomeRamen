using MyHomeRamen.Blazor.Features.Payments.Common.Services.Contracts.PaymentMethods.Responses;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Payments.Common.Services;

public sealed class PaymentApiClient(HttpClient httpClient)
{
    public async Task<IEnumerable<GetAvailableMethodsResponse>> GetAvailableMethodsAsync(
        Guid restaurantId,
        CancellationToken ct = default)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Get,
            "/api/payments/available-methods",
            restaurantId);
        using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<IEnumerable<GetAvailableMethodsResponse>>(ct)
            ?? [];
    }
}
