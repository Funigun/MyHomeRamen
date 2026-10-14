using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

public sealed class RestaurantApiClient(HttpClient httpClient, ApiService apiService)
{
    private const string BaseUrl = "api/restaurants/restaurants";
    private const string FailureMessage = "Restaurant request failed.";

    public Task<ApiResult<GetAvailableRestaurantsResponse>> GetAvailableAsync(CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => GetAvailableCoreAsync(cancellationToken));

    public Task<ApiResult<bool>> CreateAsync(CreateRestaurantRequest request, CancellationToken cancellationToken = default) =>
        apiService.ExecuteAsync(() => CreateCoreAsync(request, cancellationToken));

    private async Task<GetAvailableRestaurantsResponse> GetAvailableCoreAsync(CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(BaseUrl, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetAvailableRestaurantsResponse>(cancellationToken: cancellationToken)
            ?? throw new ApiRequestException(HttpStatusCode.InternalServerError, FailureMessage);
    }

    private async Task CreateCoreAsync(CreateRestaurantRequest request, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        await response.EnsureApiSuccessAsync(FailureMessage, cancellationToken);
    }
}
