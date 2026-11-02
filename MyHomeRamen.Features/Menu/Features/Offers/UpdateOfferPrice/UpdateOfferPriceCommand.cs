using FluentValidation;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.Offers.UpdateOfferPrice;

public sealed record UpdateOfferPriceCommand(Guid Id, decimal Price) : ICommand<UpdateOfferPriceResponse>;

public sealed class UpdateOfferPriceAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<UpdateOfferPriceCommand>
{
    public Task<bool> Authorize(UpdateOfferPriceCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageOffers,
            MenuPermissionConstants.CanEditOffer));
}

public sealed class UpdateOfferPriceValidator : AbstractValidator<UpdateOfferPriceCommand>
{
    public UpdateOfferPriceValidator(IMenuDbContext dbContext)
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

        RuleFor(command => command.Price)
            .GreaterThan(0m)
            .WithMessage("Offer price must be greater than zero.");
    }
}

public sealed class UpdateOfferPriceHandler(IMenuDbContext dbContext)
    : IRequestHandler<UpdateOfferPriceCommand, UpdateOfferPriceResponse>
{
    public async Task<UpdateOfferPriceResponse> Handle(
        UpdateOfferPriceCommand command,
        CancellationToken cancellationToken)
    {
        RestaurantProductOffer? offer = await dbContext.RestaurantProductOffer.Load()
            .ByIdForUpdate(new RestaurantProductOfferId(command.Id), cancellationToken);

        if (offer is null)
        {
            throw new InvalidOperationException("The restaurant offer could not be loaded after validation.");
        }

        offer.UpdatePrice(command.Price);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateOfferPriceResponse(offer.Id.Value);
    }
}
