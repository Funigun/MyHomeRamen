namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

public sealed record RestaurantViewModel(
    Guid Id,
    string Name,
    bool IsActive,
    string Street,
    string City,
    string ZipCode,
    string? Phone,
    string? Email)
{
    public string Address => $"{Street}, {ZipCode} {City}";

    public static RestaurantViewModel FromResponse(AvailableRestaurantResponse response)
        => new(response.Id, response.Name, response.IsActive, response.Street, response.City, response.ZipCode, response.Phone, response.Email);
}
