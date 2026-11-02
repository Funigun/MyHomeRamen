using FluentValidation;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.Offers.RetireOffer;

public sealed record RetireOfferCommand(Guid Id) : ICommand;

public sealed class RetireOfferAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<RetireOfferCommand>
{
    public Task<bool> Authorize(RetireOfferCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageOffers,
            MenuPermissionConstants.CanDeleteOffer));
}

public sealed class RetireOfferValidator : AbstractValidator<RetireOfferCommand>
{
    public RetireOfferValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Offer ID must not be empty.")
            .MustAsync(async (id, cancellationToken) =>
                await dbContext.RestaurantProductOffer.Exists(
                    offer => offer.Id == new RestaurantProductOfferId(id)
                        && offer.RestaurantId == dbContext.RestaurantId
                        && offer.Status == RestaurantProductOfferStatus.Active,
                    cancellationToken))
            .WithMessage("An active offer at this restaurant must exist.");
    }
}

public sealed class RetireOfferHandler(IMenuDbContext dbContext) : IRequestHandler<RetireOfferCommand, Unit>
{
    public async Task<Unit> Handle(RetireOfferCommand command, CancellationToken cancellationToken)
    {
        RestaurantProductOffer? offer = await dbContext.RestaurantProductOffer.Load()
            .ByIdForUpdate(new RestaurantProductOfferId(command.Id), cancellationToken);
        if (offer is null)
        {
            throw new InvalidOperationException("The restaurant offer could not be loaded.");
        }

        offer.Retire();
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
