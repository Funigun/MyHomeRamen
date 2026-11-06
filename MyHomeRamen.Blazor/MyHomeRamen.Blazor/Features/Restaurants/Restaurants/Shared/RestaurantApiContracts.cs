namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

public sealed record GetAvailableRestaurantsResponse(IReadOnlyList<AvailableRestaurantResponse> Restaurants, bool CanCreate);

public sealed record GetEmployeeRestaurantOptionsResponse(IReadOnlyList<EmployeeRestaurantOptionContract> Restaurants);

public sealed record EmployeeRestaurantOptionContract(
    Guid Id,
    string Name,
    string Street,
    string City,
    string ZipCode,
    double Latitude,
    double Longitude);

public sealed record GetCustomerRestaurantOptionsResponse(IReadOnlyList<CustomerRestaurantOptionContract> Restaurants);

public sealed record CustomerRestaurantOptionContract(
    Guid Id,
    string Name,
    string Street,
    string City,
    string ZipCode,
    double Latitude,
    double Longitude);

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
