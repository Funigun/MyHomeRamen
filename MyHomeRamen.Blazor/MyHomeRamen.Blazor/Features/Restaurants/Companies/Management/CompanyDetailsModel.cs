using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Management;

public sealed class CompanyDetailsModel
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string? LogoUrl { get; private set; } = string.Empty;

    public CompanyBusinessDetailsModel BusinessDetails { get; private set; } = default!;

    public IReadOnlyDictionary<string, bool> AllowedActions { get; private set; } = new Dictionary<string, bool>();

    public static CompanyDetailsModel FromDto(CompanyDetailsDto dto)
    {
        return new CompanyDetailsModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl,
            BusinessDetails = CompanyBusinessDetailsModel.FromDto(dto.BusinessDetails),
            AllowedActions = dto.AllowedActions.ToDictionary(dict => dict.Key, dict => dict.Value)
        };
    }
}

public sealed class CompanyBusinessDetailsModel
{
    public string LegalName { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;

    public static CompanyBusinessDetailsModel FromDto(CompanyBusinessDetailsDto dto)
    {
        return new CompanyBusinessDetailsModel
        {
            LegalName = dto.LegalName,
            TaxId = dto.TaxId
        };
    }
}
