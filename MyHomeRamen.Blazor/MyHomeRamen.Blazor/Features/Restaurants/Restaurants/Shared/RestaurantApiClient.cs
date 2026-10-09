using System.Net;

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

public sealed record GetAvailableRestaurantsResponse(IReadOnlyList<AvailableRestaurantResponse> Restaurants, bool CanCreate);

public sealed record CreateRestaurantRequest(
    string Name,
    bool IsActive,
    string Street,
    string City,
    string ZipCode,
    double Latitude,
    double Longitude,
    string? Phone,
    string? Email,
    string? AccountNumber,
    string? BankName,
    string? RoutingNumber);

public sealed record AvailableRestaurantResponse(
    Guid Id,
    string Name,
    bool IsActive,
    string Street,
    string City,
    string ZipCode,
    double Latitude,
    double Longitude,
    string? Phone,
    string? Email);

public sealed class RestaurantApiException(HttpStatusCode statusCode, string? detail = null) : Exception(detail)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
