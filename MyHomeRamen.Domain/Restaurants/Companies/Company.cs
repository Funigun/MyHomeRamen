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
        => Create(new CompanyId(Guid.CreateVersion7()), name, description, logoUrl);

    public static Company Create(CompanyId id, string name, string? description, string? logoUrl)
    {
        Company companyDetails = new()
        {
            Id = id,
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

    public void AddSocialMedia(SocialMedia socialMedia)
    {
        _media.Add(socialMedia);
    }

    public void UpdateSocialMedia(SocialMediaId socialMediaId, string name, string logoUrl, string url)
    {
        SocialMedia socialMedia = FindSocialMedia(socialMediaId);
        socialMedia.Update(name, logoUrl, url);
    }

    public void RemoveSocialMedia(SocialMediaId socialMediaId)
    {
        SocialMedia socialMedia = FindSocialMedia(socialMediaId);
        _media.Remove(socialMedia);
    }

    private SocialMedia FindSocialMedia(SocialMediaId socialMediaId)
        => _media.FirstOrDefault(media => media.Id == socialMediaId)
           ?? throw new InvalidOperationException("Social media does not belong to this company.");

    public void UpdateBusinessDetails(string legalName, string taxId)
    {
        BusinessDetails = BusinessDetails.Create(legalName, taxId);
    }
}
