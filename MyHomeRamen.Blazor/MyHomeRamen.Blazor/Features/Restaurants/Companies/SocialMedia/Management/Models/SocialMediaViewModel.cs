using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.SocialMedia.Management.Models;

public sealed class SocialMediaViewModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string LogoUrl { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;

    public static SocialMediaViewModel FromResponse(SocialMediaManageResponse response) => new()
    {
        Id = response.Id,
        Name = response.Name,
        LogoUrl = response.LogoUrl,
        Url = response.Url
    };
}
