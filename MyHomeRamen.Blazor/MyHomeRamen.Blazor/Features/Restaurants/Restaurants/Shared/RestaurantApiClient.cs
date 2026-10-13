using System.Net;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

public sealed class RestaurantApiClient(HttpClient httpClient)
{
    private const string BaseUrl = "api/restaurants/restaurants";

    public async Task<GetAvailableRestaurantsResponse> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(BaseUrl, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GetAvailableRestaurantsResponse>(cancellationToken: cancellationToken)
            ?? throw new RestaurantApiException(HttpStatusCode.InternalServerError);
    }

    public async Task CreateAsync(CreateRestaurantRequest request, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(BaseUrl, request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string detail = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new RestaurantApiException(response.StatusCode, detail);
    }
}

public sealed class RestaurantApiException(HttpStatusCode statusCode, string? detail = null)
    : ApiException(statusCode, detail ?? "Restaurant request failed.");
