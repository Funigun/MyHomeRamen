namespace MyHomeRamen.Blazor.Infrastructure.Api;

public static class RestaurantContextApiRequest
{
    public const string HeaderName = "X-Restaurant-Id";

    public static HttpRequestMessage Create(HttpMethod method, string uri, Guid restaurantId)
    {
        if (restaurantId == Guid.Empty)
        {
            throw new ArgumentException("Restaurant identifier must not be empty.", nameof(restaurantId));
        }

        HttpRequestMessage request = new(method, uri);
        request.Headers.Add(HeaderName, restaurantId.ToString());
        return request;
    }
}
