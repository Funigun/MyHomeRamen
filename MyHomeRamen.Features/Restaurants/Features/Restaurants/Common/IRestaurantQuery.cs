using MyHomeRamen.Domain.Restaurants.Restaurants;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

public interface IRestaurantQuery
{
    Task<bool> IsNameUnique(string name, CancellationToken cancellationToken);
    Task<RestaurantDetailsDto?> Details(RestaurantId restaurantId, CancellationToken cancellationToken);
    Task<IEnumerable<RestaurantListItemDto>> Available(CancellationToken cancellationToken);
}

public sealed record RestaurantDetailsDto(Guid Id, string Name, bool IsActive, string Street, string City, string ZipCode, double Latitude, double Longitude, string? Phone, string? Email, string? AccountNumber, string? BankName, string? RoutingNumber);
public sealed record RestaurantListItemDto(Guid Id, string Name, bool IsActive, string Street, string City, string ZipCode, double Latitude, double Longitude, string? Phone, string? Email);
