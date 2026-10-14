using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.CompanyDetails.Models;

public sealed class CompanyDetailsFormModel
{
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string LegalName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;

    public static CompanyDetailsFormModel FromViewModel(CompanyDetailsViewModel viewModel) => new()
    {
        Description = viewModel.Description,
        LogoUrl = viewModel.LogoUrl,
        LegalName = viewModel.BusinessDetails.LegalName,
        TaxId = viewModel.BusinessDetails.TaxId
    };

    public CompanyDetailsUpdateRequest ToRequest() => new(
        Description,
        LogoUrl,
        new BusinessDetailsForUpdateDto(LegalName, TaxId));
}
