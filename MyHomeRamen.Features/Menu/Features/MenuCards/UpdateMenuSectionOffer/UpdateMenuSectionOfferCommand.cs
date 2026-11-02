using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.MenuSectionItems;
using MyHomeRamen.Domain.Menu.MenuSections;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.UpdateMenuSectionOffer;

public sealed record UpdateMenuSectionOfferCommand(
    Guid MenuCardId,
    Guid SectionId,
    Guid ItemId,
    int SortOrder) : ICommand<UpdateMenuSectionOfferResponse>;

public sealed class UpdateMenuSectionOfferAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<UpdateMenuSectionOfferCommand>
{
    public Task<bool> Authorize(
        UpdateMenuSectionOfferCommand request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanEditMenuCard));
}

public sealed class UpdateMenuSectionOfferValidator : AbstractValidator<UpdateMenuSectionOfferCommand>
{
    public UpdateMenuSectionOfferValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command)
            .MustAsync(async (command, cancellationToken) =>
            {
                MenuCard? card = await dbContext.MenuCard.Load()
                    .ById(new MenuCardId(command.MenuCardId), cancellationToken);
                MenuSection? section = card?.Sections.FirstOrDefault(
                    candidate => candidate.Id == new MenuSectionId(command.SectionId));
                return section?.Items.Any(item => item.Id == new MenuSectionItemId(command.ItemId)) == true;
            })
            .WithMessage("The listed offer must belong to a section in this menu card.");

        RuleFor(command => command.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Menu section item sort order must not be negative.");
    }
}

public sealed class UpdateMenuSectionOfferHandler(IMenuDbContext dbContext)
    : IRequestHandler<UpdateMenuSectionOfferCommand, UpdateMenuSectionOfferResponse>
{
    public async Task<UpdateMenuSectionOfferResponse> Handle(
        UpdateMenuSectionOfferCommand command,
        CancellationToken cancellationToken)
    {
        MenuCard? card = await dbContext.MenuCard.Load()
            .ById(new MenuCardId(command.MenuCardId), cancellationToken);
        MenuSection? section = card?.Sections.FirstOrDefault(
            candidate => candidate.Id == new MenuSectionId(command.SectionId));
        if (section is null)
        {
            throw new InvalidOperationException("The menu section could not be loaded after validation.");
        }

        MenuSectionItemId itemId = new(command.ItemId);
        section.UpdateOfferSortOrder(itemId, command.SortOrder);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMenuSectionOfferResponse(command.ItemId);
    }
}
