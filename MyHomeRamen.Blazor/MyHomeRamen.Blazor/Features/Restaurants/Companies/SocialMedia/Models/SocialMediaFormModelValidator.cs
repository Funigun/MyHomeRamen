using FluentValidation;
using MyHomeRamen.Blazor.Components.Models;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.SocialMedia.Models;

public sealed class SocialMediaFormModelValidator : BaseValidator<SocialMediaFormModel>
{
    public SocialMediaFormModelValidator()
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Social media name must not be empty.")
            .MaximumLength(100).WithMessage("Social media name exceeds maximum length.");
        RuleFor(model => model.LogoUrl)
            .NotEmpty().WithMessage("Social media logo URL must not be empty.")
            .MaximumLength(2048).WithMessage("Social media logo URL exceeds maximum length.");
        RuleFor(model => model.Url)
            .NotEmpty().WithMessage("Social media URL must not be empty.")
            .MaximumLength(2048).WithMessage("Social media URL exceeds maximum length.");
    }
}
