namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.SocialMedia.Management.Models;

public sealed class SocialMediaFormModel
{
    public Guid? Id { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    public string? LogoUrl { get; private set; } = default!;

    public static SocialMediaFormModel FromViewModel(SocialMediaViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Name = viewModel.Name,
        LogoUrl = viewModel.LogoUrl
    };
}
