using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Restaurants.Companies;

public sealed class SocialMedia : IEntity<SocialMediaId>
{
    public SocialMediaId Id { get; private set; } = default!;

    public string Name { get; private set; } = string.Empty;

    public string LogoUrl { get; private set; } = string.Empty;

    public string Url { get; private set; } = string.Empty;

    private SocialMedia() { }

    public static SocialMedia Create(string name, string logoUrl, string url)
    {
        SocialMedia socialMedia = new()
        {
            Id = new SocialMediaId(Guid.CreateVersion7()),
            Name = name,
            LogoUrl = logoUrl,
            Url = url
        };

        SocialMediaValidator.Validate(socialMedia);
        return socialMedia;
    }
}
