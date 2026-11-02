using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.MenuSectionItems;
using MyHomeRamen.Domain.Menu.MenuSections;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.AddMenuSectionOffer;

public sealed record AddMenuSectionOfferCommand(
    Guid MenuCardId,
    Guid SectionId,
    Guid OfferId,
    int SortOrder) : ICommand<AddMenuSectionOfferResponse>;

public sealed class AddMenuSectionOfferAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<AddMenuSectionOfferCommand>
{
    public Task<bool> Authorize(AddMenuSectionOfferCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanEditMenuCard));
}

public sealed class AddMenuSectionOfferValidator : AbstractValidator<AddMenuSectionOfferCommand>
{
    public AddMenuSectionOfferValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command)
            .MustAsync(async (command, cancellationToken) =>
            {
                MenuCard? card = await dbContext.MenuCard.Load()
                    .ById(new MenuCardId(command.MenuCardId), cancellationToken);
                bool sectionExists = card?.Sections.Any(
                    section => section.Id == new MenuSectionId(command.SectionId)) == true;
                bool offerExists = await dbContext.RestaurantProductOffer.Load()
                    .ByIdForUpdate(new RestaurantProductOfferId(command.OfferId), cancellationToken) is not null;
                bool offerAlreadyListed = card?.Sections
                    .FirstOrDefault(section => section.Id == new MenuSectionId(command.SectionId))?
                    .Items.Any(item => item.RestaurantProductOfferId == new RestaurantProductOfferId(command.OfferId)) == true;

                return sectionExists && offerExists && !offerAlreadyListed;
            })
            .WithMessage("The section and active restaurant offer must exist, and offer must not already be listed.");

        RuleFor(command => command.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Menu section item sort order must not be negative.");
    }
}

public sealed class AddMenuSectionOfferHandler(IMenuDbContext dbContext)
    : IRequestHandler<AddMenuSectionOfferCommand, AddMenuSectionOfferResponse>
{
    public async Task<AddMenuSectionOfferResponse> Handle(
        AddMenuSectionOfferCommand command,
        CancellationToken cancellationToken)
    {
        MenuCard? card = await dbContext.MenuCard.Load()
            .ById(new MenuCardId(command.MenuCardId), cancellationToken);
        MenuSection? section = card?.Sections.FirstOrDefault(
            candidate => candidate.Id == new MenuSectionId(command.SectionId));
        RestaurantProductOffer? offer = await dbContext.RestaurantProductOffer.Load()
            .ByIdForUpdate(new RestaurantProductOfferId(command.OfferId), cancellationToken);
        if (section is null || offer is null)
        {
            throw new InvalidOperationException("The section or offer could not be loaded after validation.");
        }

        MenuSectionItem item = section.AddOffer(
            new MenuSectionItemId(Guid.CreateVersion7()),
            offer,
            command.SortOrder);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new AddMenuSectionOfferResponse(item.Id.Value);
    }
}
