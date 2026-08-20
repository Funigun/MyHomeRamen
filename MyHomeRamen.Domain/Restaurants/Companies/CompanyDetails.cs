using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Restaurants.Companies.ValueObjects;

namespace MyHomeRamen.Domain.Restaurants.Companies;

public sealed class CompanyDetails : Aggregate<CompanyDetailsId>
{
    private readonly List<SocialMedia> _media = [];

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; } = string.Empty;

    public string? LogoUrl { get; private set; } = string.Empty;

    public BusinessDetails BusinessDetails { get; private set; } = default!;

    public IReadOnlyList<SocialMedia> Media => _media.ToList();

    private CompanyDetails() { }

    public static CompanyDetails Create(string name, string? description, string? logoUrl)
    {
        CompanyDetails companyDetails = new()
        {
            Id = new CompanyDetailsId(Guid.CreateVersion7()),
            Name = name,
            Description = description,
            LogoUrl = logoUrl
        };

        CompanyDetailsValidator.Validate(companyDetails);
        return companyDetails;
    }

    public void UpdateBusinessDetails(string legalName, string taxId)
    {
        BusinessDetails = BusinessDetails.Create(legalName, taxId);
    }
}
