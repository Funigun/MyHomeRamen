using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.MenuSections;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.DeleteMenuSection;

public sealed record DeleteMenuSectionCommand(Guid MenuCardId, Guid SectionId) : ICommand;

public sealed class DeleteMenuSectionAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<DeleteMenuSectionCommand>
{
    public Task<bool> Authorize(DeleteMenuSectionCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanDeleteMenuCard));
}

public sealed class DeleteMenuSectionValidator : AbstractValidator<DeleteMenuSectionCommand>
{
    public DeleteMenuSectionValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command)
            .MustAsync(async (command, cancellationToken) =>
            {
                MenuCard? card = await dbContext.MenuCard.Load()
                    .ById(new MenuCardId(command.MenuCardId), cancellationToken);
                return card?.Sections.Any(section => section.Id == new MenuSectionId(command.SectionId)) == true;
            })
            .WithMessage("The menu section must belong to a menu card at this restaurant.");
    }
}

public sealed class DeleteMenuSectionHandler(IMenuDbContext dbContext)
    : IRequestHandler<DeleteMenuSectionCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMenuSectionCommand command, CancellationToken cancellationToken)
    {
        MenuCard? card = await dbContext.MenuCard.Load()
            .ById(new MenuCardId(command.MenuCardId), cancellationToken);
        if (card is null || !card.RemoveSection(new MenuSectionId(command.SectionId)))
        {
            throw new InvalidOperationException("The menu section could not be removed after validation.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
