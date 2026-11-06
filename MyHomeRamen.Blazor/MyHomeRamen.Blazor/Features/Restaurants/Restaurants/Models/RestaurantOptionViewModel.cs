using MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Models;

public sealed record RestaurantOptionViewModel(
    Guid Id,
    string Name,
    string Street,
    string City,
    string ZipCode,
    double Latitude,
    double Longitude)
{
    public string Address => $"{Street}, {ZipCode} {City}";

    public static RestaurantOptionViewModel FromEmployeeResponse(EmployeeRestaurantOptionContract response)
        => new(response.Id, response.Name, response.Street, response.City, response.ZipCode, response.Latitude, response.Longitude);

    public static RestaurantOptionViewModel FromCustomerResponse(CustomerRestaurantOptionContract response)
        => new(response.Id, response.Name, response.Street, response.City, response.ZipCode, response.Latitude, response.Longitude);
}
