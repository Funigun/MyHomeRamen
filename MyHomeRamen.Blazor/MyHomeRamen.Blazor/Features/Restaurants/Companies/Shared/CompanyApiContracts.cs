namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

public sealed record CompanyDetailsDto(Guid Id, string Name, string Description, string? LogoUrl, CompanyBusinessDetailsDto BusinessDetails, Dictionary<string, bool> AllowedActions);
public sealed record CompanyBusinessDetailsDto(string LegalName, string TaxId);

public sealed record CompanySocialMediaDto(Guid Id, string Name, IEnumerable<SocialMediaDto> SocialMedia, Dictionary<string, bool> AllowedActions);
public sealed record SocialMediaDto(Guid Id, string Name, string? LogoUrl, string? Url, Dictionary<string, bool> AllowedActions);

public sealed record CompanyRestaurantsDto(Guid Id, string Name, IEnumerable<RestaurantDto> Restaurants, Dictionary<string, bool> AllowedActions);
public sealed record RestaurantDto(Guid Id, string Name, string? LogoUrl, RestaurantAddressDto Address, RestaurantContactDto Contact, Dictionary<string, bool> AllowedActions);
public sealed record RestaurantAddressDto(string Street, string City, string ZipCode);
public sealed record RestaurantContactDto(string PhoneNumber, string Email);

public sealed record RegisterCompanyOwnerRequest(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    string CompanyName);

public sealed record RegisterCompanyOwnerResponse(Guid CompanyId);
