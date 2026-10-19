namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

public sealed record CompanyDetailsResponse(Guid Id, string Name, string Description, string? LogoUrl, BusinessDetailsDto BusinessDetails, CompanyDetailsAllowedActions AllowedActions);
public sealed record CompanyDetailsAllowedActions(bool CanEditCompanyDetails);
public sealed record BusinessDetailsDto(string LegalName, string TaxId);
public sealed record CompanyDetailsUpdateRequest(string Description, string? LogoUrl, BusinessDetailsForUpdateDto BusinessDetails);
public sealed record BusinessDetailsForUpdateDto(string LegalName, string TaxId);

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
