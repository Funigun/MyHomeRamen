namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

public sealed record CompanyDetailsResponse(Guid Id, string Name, string Description, string? LogoUrl, BusinessDetailsDto BusinessDetails, CompanyDetailsAllowedActions AllowedActions);
public sealed record CompanyDetailsAllowedActions(bool CanEditCompanyDetails);
public sealed record BusinessDetailsDto(string LegalName, string TaxId);
public sealed record CompanyDetailsUpdateRequest(string Description, string? LogoUrl, BusinessDetailsForUpdateDto BusinessDetails);
public sealed record BusinessDetailsForUpdateDto(string LegalName, string TaxId);
public sealed record CompanySetupStatusResponse(bool IsOwnerConfigured, bool CompanyExists, bool RestaurantExists)
{
    public bool IsComplete => IsOwnerConfigured && CompanyExists && RestaurantExists;
}

public sealed record RegisterCompanyOwnerRequest(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword);

public sealed record ConfigureCompanyRequest(string Name, string LegalName, string TaxId);

public sealed record InitialRestaurantSetupRequest(
    string Name,
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
