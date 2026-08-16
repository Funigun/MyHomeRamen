using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Venues.Companies;

public sealed class SocialMedia : IEntity<SocialMediaId>
{
    public SocialMediaId Id { get; private set; } = default!;

    public string Name { get; private set; } = string.Empty;

    public string LogoUrl { get; private set; } = string.Empty;

    public string Url { get; private set; } = string.Empty;

    private SocialMedia() { }

    public static SocialMedia Create(string name, string logoUrl, string url)
    {
        return new SocialMedia
        {
            Id = new SocialMediaId(Guid.CreateVersion7()),
            Name = name,
            LogoUrl = logoUrl,
            Url = url
        };
    }
}
