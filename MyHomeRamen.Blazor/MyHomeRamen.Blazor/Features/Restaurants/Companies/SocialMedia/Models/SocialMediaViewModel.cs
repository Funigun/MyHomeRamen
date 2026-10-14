using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.SocialMedia.Models;

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

    public static IReadOnlyList<SocialMediaViewModel> FromResponse(IEnumerable<SocialMediaManageResponse> response)
        => response.Select(FromResponse).ToArray();

    public static (IReadOnlyList<SocialMediaViewModel> Items, bool CanEdit) FromResponse(GetSocialMediaForManageResponse response)
        => (FromResponse(response.Items), response.CanEditMedia);
}
