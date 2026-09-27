namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

public sealed class RestaurantApiClient(HttpClient httpClient)
{
    private const string BASE_URL = "api/restaurants";
}
