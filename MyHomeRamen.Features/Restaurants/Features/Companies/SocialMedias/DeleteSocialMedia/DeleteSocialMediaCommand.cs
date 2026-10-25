using FluentValidation;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.SocialMedias.DeleteSocialMedia;

public sealed record DeleteSocialMediaCommand(Guid SocialMediaId) : ICommand<Unit>;

public sealed class DeleteSocialMediaAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<DeleteSocialMediaCommand>
{
    public async Task<bool> Authorize(DeleteSocialMediaCommand request, CancellationToken cancellationToken)
        => await Task.FromResult(currentUser.CanEditCompanySocialMedia());
}

public sealed class DeleteSocialMediaValidator : AbstractValidator<DeleteSocialMediaCommand>
{
    public DeleteSocialMediaValidator(IRestaurantDbContext restaurantDbContext)
    {
        RuleFor(x => x.SocialMediaId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Social media ID must not be empty.")
            .MustAsync(async (command, socialMediaId, cancellationToken) =>
            {
                SocialMediaId mediaId = socialMediaId;
                return await restaurantDbContext.Company.Query().HasMedia(mediaId, cancellationToken);
            })
            .WithMessage("Social media with the specified ID does not exist.");

    }
}

public sealed class DeleteSocialMediaHandler(IRestaurantDbContext dbContext) : IRequestHandler<DeleteSocialMediaCommand, Unit>
{
    public async Task<Unit> Handle(DeleteSocialMediaCommand command, CancellationToken cancellationToken)
    {
        Company company = await dbContext.Company.Load().Single(cancellationToken);
        
        company.RemoveSocialMedia(command.SocialMediaId);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
