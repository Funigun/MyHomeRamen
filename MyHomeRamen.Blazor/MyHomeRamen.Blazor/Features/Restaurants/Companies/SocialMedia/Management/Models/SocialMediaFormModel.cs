using FluentValidation;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.SocialMedia.Management.Models;

public sealed class SocialMediaFormModel
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public static SocialMediaFormModel FromViewModel(SocialMediaViewModel viewModel) => new()
    {
        Id = viewModel.Id,
        Name = viewModel.Name,
        LogoUrl = viewModel.LogoUrl,
        Url = viewModel.Url
    };

    public CreateSocialMediaRequest ToCreateRequest() => new(Name, LogoUrl, Url);
    public UpdateSocialMediaRequest ToUpdateRequest() => new(Name, LogoUrl, Url);
}

public sealed class SocialMediaFormModelValidator : BaseValidator<SocialMediaFormModel>
{
    public SocialMediaFormModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Social media name must not be empty.")
            .MaximumLength(100).WithMessage("Social media name exceeds maximum length.");
        RuleFor(x => x.LogoUrl)
            .NotEmpty().WithMessage("Social media logo URL must not be empty.")
            .MaximumLength(2048).WithMessage("Social media logo URL exceeds maximum length.");
        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Social media URL must not be empty.")
            .MaximumLength(2048).WithMessage("Social media URL exceeds maximum length.");
    }
}
