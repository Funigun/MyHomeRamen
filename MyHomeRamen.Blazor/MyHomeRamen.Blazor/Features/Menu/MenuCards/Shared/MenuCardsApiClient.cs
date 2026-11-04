using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Menu.MenuCards.Shared;

public sealed class MenuCardsApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string BaseUrl = "api/menu/cards";
    private const string FailureMessage = "Menu card request failed.";

    public Task<ApiResult<GetMenuCardOptionsResponse>> GetOptionsAsync(
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => GetOptionsCoreAsync(cancellationToken));

    public Task<ApiResult<MenuCardResponseContract>> GetByIdAsync(
        Guid menuCardId,
        Guid restaurantId,
        CancellationToken cancellationToken = default)
        => apiService.ExecuteAsync(() => GetByIdCoreAsync(menuCardId, restaurantId, cancellationToken));

    private async Task<GetMenuCardOptionsResponse> GetOptionsCoreAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync($"{BaseUrl}/options", cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetMenuCardOptionsResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task<MenuCardResponseContract> GetByIdCoreAsync(
        Guid menuCardId,
        Guid restaurantId,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = RestaurantContextApiRequest.Create(
            HttpMethod.Get,
            $"{BaseUrl}/{menuCardId}",
            restaurantId);
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<MenuCardResponseContract>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }
}
