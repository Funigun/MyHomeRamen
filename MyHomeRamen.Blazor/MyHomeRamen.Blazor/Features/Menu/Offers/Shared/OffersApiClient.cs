using System.Net;
using MyHomeRamen.Blazor.Features.Menu.MenuCards.Shared;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Menu.Offers.Shared;

public sealed class OffersApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string BaseUrl = "api/menu/offers";
    private const string FailureMessage = "Offer request failed.";

    public Task<ApiResult<GetOfferCustomizationResponse>> GetForCustomizationAsync(
        Guid offerId,
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => GetForCustomizationCoreAsync(offerId, restaurantId, cancellationToken));

    private async Task<GetOfferCustomizationResponse> GetForCustomizationCoreAsync(
        Guid offerId,
        Guid restaurantId,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Get,
            $"{BaseUrl}/{offerId}",
            restaurantId);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetOfferCustomizationResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }
}
