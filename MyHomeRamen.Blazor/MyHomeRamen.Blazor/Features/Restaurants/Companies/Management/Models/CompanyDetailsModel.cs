using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Management.Models;

public sealed class CompanyDetailsModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public CompanyBusinessDetailsModel BusinessDetails { get; init; } = new(string.Empty, string.Empty);
    public IReadOnlyDictionary<string, bool> AllowedActions { get; init; } = new Dictionary<string, bool>();
    public bool CanEditCompanyDetails => AllowedActions.TryGetValue("canEditCompanyDetails", out bool allowed) && allowed;

    public static CompanyDetailsModel FromResponse(CompanyDetailsResponse response) => new()
    {
        Id = response.Id,
        Name = response.Name,
        Description = response.Description,
        LogoUrl = response.LogoUrl,
        BusinessDetails = CompanyBusinessDetailsModel.FromResponse(response.BusinessDetails),
        AllowedActions = response.AllowedActions.ToDictionary(item => item.Key, item => item.Value)
    };
}

public sealed record CompanyBusinessDetailsModel(string LegalName, string TaxId)
{
    public static CompanyBusinessDetailsModel FromResponse(BusinessDetailsDto response)
        => new(response.LegalName, response.TaxId);
}
