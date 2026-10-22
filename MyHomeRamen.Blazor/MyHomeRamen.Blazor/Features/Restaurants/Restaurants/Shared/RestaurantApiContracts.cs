namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

public sealed record GetAvailableRestaurantsResponse(IReadOnlyList<AvailableRestaurantResponse> Restaurants, bool CanCreate);

public sealed record CreateRestaurantRequest(
    string Name,
    bool IsActive,
    string Street,
    string City,
    string ZipCode,
    double Latitude,
    double Longitude,
    string Phone,
    string Email,
    string AccountNumber,
    string BankName,
    string RoutingNumber);

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
