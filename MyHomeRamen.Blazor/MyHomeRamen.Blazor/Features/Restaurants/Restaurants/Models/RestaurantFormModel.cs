using MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Models;

public sealed class CreateRestaurantFormModel
{
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string AccountNumber { get; set; } = string.Empty;

    public string BankName { get; set; } = string.Empty;

    public string RoutingNumber { get; set; } = string.Empty;

    public CreateRestaurantRequest ToRequest()
        => new(Name, IsActive, Street, City, ZipCode, Latitude!.Value, Longitude!.Value,
            Phone, Email, AccountNumber, BankName, RoutingNumber);
}
