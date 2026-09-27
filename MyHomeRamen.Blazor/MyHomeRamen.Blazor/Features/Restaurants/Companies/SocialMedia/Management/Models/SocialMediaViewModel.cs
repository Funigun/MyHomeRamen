using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.SocialMedia.Management.Models;

public class SocialMediaViewModel
{
    public Guid? Id { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    public string? LogoUrl { get; private set; } = default!;

    public Dictionary<string, bool> AllowedActions { get; private set; } = default!;

    public static SocialMediaViewModel FromDto(SocialMediaDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        LogoUrl = dto.LogoUrl,
        AllowedActions = dto.AllowedActions
    };
}
