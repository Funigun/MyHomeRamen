using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.SocialMedia.Models;

public sealed class SocialMediaFormModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public CreateSocialMediaRequest ToCreateRequest() => new(Name, LogoUrl, Url);
    public UpdateSocialMediaRequest ToUpdateRequest() => new(Name, LogoUrl, Url);

    public static SocialMediaFormModel FromViewModel(SocialMediaViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Name = viewModel.Name,
        LogoUrl = viewModel.LogoUrl,
        Url = viewModel.Url
    };
}
