using FluentValidation;
using MyHomeRamen.Domain.Common.SocialMedia;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.SocialMedias.CreateSocialMedia;

public sealed record CreateSocialMediaRequest(string Name, string LogoUrl, string Url);
public sealed record CreateSocialMediaCommand(CreateSocialMediaRequest Request) : ICommand<Unit>;

public sealed class CreateSocialMediaAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<CreateSocialMediaCommand>
{
    public async Task<bool> Authorize(CreateSocialMediaCommand request, CancellationToken cancellationToken)
        => await Task.FromResult(currentUser.CanEditCompanySocialMedia());
}

public sealed class CreateSocialMediaValidator : AbstractValidator<CreateSocialMediaCommand>
{
    public CreateSocialMediaValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
                .WithMessage("Social media name must not be empty.")
            .MaximumLength(SocialMediaConstants.MaxNameLength)
                .WithMessage("Social media name exceeds maximum length.");

        RuleFor(x => x.Request.LogoUrl)
            .NotEmpty()
                .WithMessage("Social media logo URL must not be empty.")
            .MaximumLength(SocialMediaConstants.MaxLogoUrlLength)
                .WithMessage("Social media logo URL exceeds maximum length.");

        RuleFor(x => x.Request.Url)
            .NotEmpty()
                .WithMessage("Social media URL must not be empty.")
             .MaximumLength(SocialMediaConstants.MaxUrlLength)
                .WithMessage("Social media URL exceeds maximum length.");
    }
}

public sealed class CreateSocialMediaHandler(IRestaurantDbContext dbContext) : IRequestHandler<CreateSocialMediaCommand, Unit>
{
    public async Task<Unit> Handle(CreateSocialMediaCommand command, CancellationToken cancellationToken)
    {
        Company company = await dbContext.Company.Load().Single(cancellationToken);
        
        company.AddSocialMedia(SocialMedia.Create(command.Request.Name, command.Request.LogoUrl, command.Request.Url));
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
