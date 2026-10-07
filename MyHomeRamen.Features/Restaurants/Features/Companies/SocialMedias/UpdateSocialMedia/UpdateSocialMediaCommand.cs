using FluentValidation;
using MyHomeRamen.Domain.Common.SocialMedia;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Domain.Restaurants.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.SocialMedias.UpdateSocialMedia;

public sealed record UpdateSocialMediaRequest(string Name, string LogoUrl, string Url);
public sealed record UpdateSocialMediaCommand(Guid SocialMediaId, UpdateSocialMediaRequest Request) : ICommand<Unit>;

public sealed class UpdateSocialMediaAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<UpdateSocialMediaCommand>
{
    public async Task<bool> Authorize(UpdateSocialMediaCommand request, CancellationToken cancellationToken) 
        => await Task.FromResult(currentUser.Permissions.Contains(PermissionConstants.CompanySocialMediaEdit));
}

public sealed class UpdateSocialMediaValidator : AbstractValidator<UpdateSocialMediaCommand>
{
    public UpdateSocialMediaValidator()
    {
        RuleFor(x => x.SocialMediaId)
            .NotEmpty()
                .WithMessage("Social media ID must not be empty.");

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

public sealed class UpdateSocialMediaHandler(IRestaurantDbContext dbContext) : IRequestHandler<UpdateSocialMediaCommand, Unit>
{
    public async Task<Unit> Handle(UpdateSocialMediaCommand command, CancellationToken cancellationToken)
    {
        Company company = await dbContext.Company.Load().Single(cancellationToken);
        company.UpdateSocialMedia(command.SocialMediaId, command.Request.Name, command.Request.LogoUrl, command.Request.Url);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
