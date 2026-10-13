using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Management.Models;

public sealed class CompanyDetailsEditModel
{
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string LegalName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;

    public static CompanyDetailsEditModel FromDetails(CompanyDetailsModel details) => new()
    {
        Description = details.Description,
        LogoUrl = details.LogoUrl,
        LegalName = details.BusinessDetails.LegalName,
        TaxId = details.BusinessDetails.TaxId
    };

    public CompanyDetailsUpdateRequest ToRequest() => new(
        Description,
        LogoUrl,
        new BusinessDetailsForUpdateDto(LegalName, TaxId));
}
