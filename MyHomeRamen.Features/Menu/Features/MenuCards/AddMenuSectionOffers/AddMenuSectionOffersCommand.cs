using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.MenuSectionItems;
using MyHomeRamen.Domain.Menu.MenuSections;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.AddMenuSectionOffers;

public sealed record AddMenuSectionOffersCommand(
    Guid MenuCardId,
    Guid SectionId,
    IReadOnlyList<Guid> OfferIds) : ICommand<AddMenuSectionOffersResponse>;

public sealed class AddMenuSectionOffersAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<AddMenuSectionOffersCommand>
{
    public Task<bool> Authorize(
        AddMenuSectionOffersCommand request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanEditMenuCard));
}

public sealed class AddMenuSectionOffersValidator : AbstractValidator<AddMenuSectionOffersCommand>
{
    public const int MaxOfferCount = 100;

    public AddMenuSectionOffersValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command.OfferIds)
            .NotEmpty()
            .WithMessage("At least one offer ID must be provided.")
            .Must(offerIds => offerIds is not null && offerIds.Count <= MaxOfferCount)
            .WithMessage($"No more than {MaxOfferCount} offers may be assigned at once.")
            .Must(offerIds => offerIds is not null && offerIds.Distinct().Count() == offerIds.Count)
            .WithMessage("Offer IDs must be unique.");

        RuleFor(command => command)
            .MustAsync(async (command, cancellationToken) =>
            {
                if (command.OfferIds is null
                    || command.OfferIds.Count == 0
                    || command.OfferIds.Count > MaxOfferCount
                    || command.OfferIds.Distinct().Count() != command.OfferIds.Count)
                {
                    return false;
                }

                MenuCard? card = await dbContext.MenuCard.Load()
                    .ById(new MenuCardId(command.MenuCardId), cancellationToken);
                MenuSection? section = card?.Sections.FirstOrDefault(
                    candidate => candidate.Id == new MenuSectionId(command.SectionId));
                if (card is null || card.RestaurantId != dbContext.RestaurantId || section is null)
                {
                    return false;
                }

                HashSet<RestaurantProductOfferId> requestedOfferIds = command.OfferIds
                    .Select(id => new RestaurantProductOfferId(id))
                    .ToHashSet();
                if (section.Items.Any(item => requestedOfferIds.Contains(item.RestaurantProductOfferId)))
                {
                    return false;
                }

                IReadOnlyCollection<RestaurantProductOffer> offers = await dbContext.RestaurantProductOffer.Load()
                    .ByIdsForUpdate(requestedOfferIds, cancellationToken);

                return offers.Count == requestedOfferIds.Count
                    && offers.All(offer =>
                        offer.RestaurantId == dbContext.RestaurantId
                        && offer.Status == RestaurantProductOfferStatus.Active
                        && offer.Product.Status == ProductStatus.Active);
            })
            .WithMessage("The menu card, section, and all selected active restaurant offers must be valid and offers must not already be listed.");
    }
}

public sealed class AddMenuSectionOffersHandler(IMenuDbContext dbContext)
    : IRequestHandler<AddMenuSectionOffersCommand, AddMenuSectionOffersResponse>
{
    public async Task<AddMenuSectionOffersResponse> Handle(
        AddMenuSectionOffersCommand command,
        CancellationToken cancellationToken)
    {
        MenuCard? card = await dbContext.MenuCard.Load()
            .ById(new MenuCardId(command.MenuCardId), cancellationToken);
        MenuSection? section = card?.Sections.FirstOrDefault(
            candidate => candidate.Id == new MenuSectionId(command.SectionId));
        if (card is null || card.RestaurantId != dbContext.RestaurantId || section is null)
        {
            throw new InvalidOperationException("The menu section could not be loaded after validation.");
        }

        IReadOnlyCollection<RestaurantProductOffer> loadedOffers = await dbContext.RestaurantProductOffer.Load()
            .ByIdsForUpdate(
                command.OfferIds.Select(id => new RestaurantProductOfferId(id)).ToHashSet(),
                cancellationToken);
        Dictionary<RestaurantProductOfferId, RestaurantProductOffer> offersById = loadedOffers
            .ToDictionary(offer => offer.Id);
        if (offersById.Count != command.OfferIds.Count
            || command.OfferIds.Distinct().Count() != command.OfferIds.Count)
        {
            throw new InvalidOperationException("The selected offers could not be loaded after validation.");
        }

        int nextSortOrder = section.Items.Count == 0
            ? 0
            : checked(section.Items.Max(item => item.SortOrder) + 1);
        List<MenuSectionOfferAssignmentDto> assignments = [];
        foreach (Guid offerId in command.OfferIds)
        {
            RestaurantProductOffer offer = offersById[new RestaurantProductOfferId(offerId)];
            if (offer.RestaurantId != dbContext.RestaurantId
                || offer.Status != RestaurantProductOfferStatus.Active
                || offer.Product.Status != ProductStatus.Active)
            {
                throw new InvalidOperationException("A selected offer is no longer available for assignment.");
            }

            int sortOrder = nextSortOrder;
            MenuSectionItem item = section.AddOffer(
                new MenuSectionItemId(Guid.CreateVersion7()),
                offer,
                sortOrder);
            assignments.Add(new MenuSectionOfferAssignmentDto(offer.Id.Value, item.Id.Value, sortOrder));
            nextSortOrder = checked(nextSortOrder + 1);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new AddMenuSectionOffersResponse(assignments);
    }
}
