using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Management;

public sealed class CompanyDetailsModel
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? LogoUrl { get; private set; }
    public CompanyBusinessDetailsModel BusinessDetails { get; private set; } = new();
    public IReadOnlyDictionary<string, bool> AllowedActions { get; private set; } = new Dictionary<string, bool>();
    public bool CanEditCompanyDetails => AllowedActions.TryGetValue("canEditCompanyDetails", out bool allowed) && allowed;

    public static CompanyDetailsModel FromDto(CompanyDetailsDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        LogoUrl = dto.LogoUrl,
        BusinessDetails = CompanyBusinessDetailsModel.FromDto(dto.BusinessDetails),
        AllowedActions = dto.AllowedActions.ToDictionary(item => item.Key, item => item.Value)
    };
}

public sealed class CompanyBusinessDetailsModel
{
    public string LegalName { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;

    public static CompanyBusinessDetailsModel FromDto(CompanyBusinessDetailsDto dto) => new()
    {
        LegalName = dto.LegalName,
        TaxId = dto.TaxId
    };
}

public sealed class CompanyDetailsFormModel
{
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string LegalName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;

    public static CompanyDetailsFormModel FromDetails(CompanyDetailsModel details) => new()
    {
        Description = details.Description,
        LogoUrl = details.LogoUrl,
        LegalName = details.BusinessDetails.LegalName,
        TaxId = details.BusinessDetails.TaxId
    };

    public UpdateCompanyDetailsRequest ToRequest() => new(
        Description,
        LogoUrl,
        new BusinessDetailsForUpdateDto(LegalName, TaxId));
}
