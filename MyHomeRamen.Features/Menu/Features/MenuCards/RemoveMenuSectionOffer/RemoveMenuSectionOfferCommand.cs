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

namespace MyHomeRamen.Features.Menu.Features.MenuCards.RemoveMenuSectionOffer;

public sealed record RemoveMenuSectionOfferCommand(Guid MenuCardId, Guid SectionId, Guid ItemId) : ICommand;

public sealed class RemoveMenuSectionOfferAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<RemoveMenuSectionOfferCommand>
{
    public Task<bool> Authorize(RemoveMenuSectionOfferCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanDeleteMenuCard));
}

public sealed class RemoveMenuSectionOfferValidator : AbstractValidator<RemoveMenuSectionOfferCommand>
{
    public RemoveMenuSectionOfferValidator(IMenuDbContext dbContext)
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
    }
}

public sealed class RemoveMenuSectionOfferHandler(IMenuDbContext dbContext)
    : IRequestHandler<RemoveMenuSectionOfferCommand, Unit>
{
    public async Task<Unit> Handle(
        RemoveMenuSectionOfferCommand command,
        CancellationToken cancellationToken)
    {
        MenuCard? card = await dbContext.MenuCard.Load()
            .ById(new MenuCardId(command.MenuCardId), cancellationToken);
        MenuSection? section = card?.Sections.FirstOrDefault(
            candidate => candidate.Id == new MenuSectionId(command.SectionId));
        if (section is null || !section.RemoveOffer(new MenuSectionItemId(command.ItemId)))
        {
            throw new InvalidOperationException("The listed offer could not be removed after validation.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
