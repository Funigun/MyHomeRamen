using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.DeactivateMenuCard;

public sealed record DeactivateMenuCardCommand(Guid Id) : ICommand;

public sealed class DeactivateMenuCardAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<DeactivateMenuCardCommand>
{
    public Task<bool> Authorize(DeactivateMenuCardCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanDeleteMenuCard));
}

public sealed class DeactivateMenuCardValidator : AbstractValidator<DeactivateMenuCardCommand>
{
    public DeactivateMenuCardValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Menu card ID must not be empty.")
            .MustAsync(async (id, cancellationToken) =>
                await dbContext.MenuCard.Exists(card => card.Id == new MenuCardId(id), cancellationToken))
            .WithMessage("A menu card at this restaurant must exist.");
    }
}

public sealed class DeactivateMenuCardHandler(IMenuDbContext dbContext)
    : IRequestHandler<DeactivateMenuCardCommand, Unit>
{
    public async Task<Unit> Handle(DeactivateMenuCardCommand command, CancellationToken cancellationToken)
    {
        MenuCard? card = await dbContext.MenuCard.Load()
            .ById(new MenuCardId(command.Id), cancellationToken);
        if (card is null)
        {
            throw new InvalidOperationException("The menu card could not be loaded after validation.");
        }

        card.SetActive(false);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
