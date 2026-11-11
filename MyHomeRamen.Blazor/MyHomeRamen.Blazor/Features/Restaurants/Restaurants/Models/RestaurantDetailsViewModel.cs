using MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Models;

public sealed record RestaurantDetailsViewModel(
    Guid Id,
    string Name,
    bool IsActive,
    string Street,
    string City,
    string ZipCode,
    string? Phone,
    string? Email)
{
    public static RestaurantDetailsViewModel FromResponse(RestaurantDetailsResponse response)
        => new(
            response.Id,
            response.Name,
            response.IsActive,
            response.Street,
            response.City,
            response.ZipCode,
            response.Phone,
            response.Email);
}
