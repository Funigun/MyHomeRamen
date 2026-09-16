using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Restaurants.Companies.ValueObjects;

namespace MyHomeRamen.Domain.Restaurants.Companies;

public sealed class Company : Aggregate<CompanyId>
{
    private readonly List<SocialMedia> _media = [];

    public string Name { get; private set; } = string.Empty;

    public string NormalizedName { get; private set; } = string.Empty;

    public string? Description { get; private set; } = string.Empty;

    public string? LogoUrl { get; private set; } = string.Empty;

    public BusinessDetails BusinessDetails { get; private set; } = default!;

    public IReadOnlyList<SocialMedia> Media => _media.ToList();

    private Company() { }

    public static Company Create(string name, string? description, string? logoUrl)
    {
        Company companyDetails = new()
        {
            Id = new CompanyId(Guid.CreateVersion7()),
            Name = name,
            NormalizedName = NormalizeName(name),
            Description = description,
            LogoUrl = logoUrl
        };

        CompanyValidator.Validate(companyDetails);
        return companyDetails;
    }

    public static string NormalizeName(string name)
    {
        return string.Concat(name.Where(char.IsLetterOrDigit)).ToUpperInvariant();
    }

    public void UpdateBusinessDetails(string legalName, string taxId)
    {
        BusinessDetails = BusinessDetails.Create(legalName, taxId);
    }
}
