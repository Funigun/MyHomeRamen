using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.ActivateMenuCard;

public sealed record ActivateMenuCardCommand(Guid Id) : ICommand<ActivateMenuCardResponse>;

public sealed class ActivateMenuCardAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<ActivateMenuCardCommand>
{
    public Task<bool> Authorize(
        ActivateMenuCardCommand request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanEditMenuCard));
}

public sealed class ActivateMenuCardValidator : AbstractValidator<ActivateMenuCardCommand>
{
    public ActivateMenuCardValidator(IMenuDbContext dbContext, ICompanyContext companyContext)
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Menu card ID must not be empty.")
            .MustAsync(async (id, cancellationToken) =>
            {
                Guid? restaurantId = companyContext.RestaurantId;
                return restaurantId.HasValue
                    && await dbContext.MenuCard.Exists(
                        card => card.Id == new MenuCardId(id)
                            && card.RestaurantId == restaurantId.Value
                            && !card.IsActive,
                        cancellationToken);
            })
            .WithMessage("An inactive menu card at the selected restaurant must exist.");
    }
}

public sealed class ActivateMenuCardHandler(IMenuDbContext dbContext, ICompanyContext companyContext)
    : IRequestHandler<ActivateMenuCardCommand, ActivateMenuCardResponse>
{
    public async Task<ActivateMenuCardResponse> Handle(
        ActivateMenuCardCommand command,
        CancellationToken cancellationToken)
    {
        Guid restaurantId = companyContext.RestaurantId
            ?? throw new InvalidOperationException("A restaurant context is required for menu card activation.");
        MenuCard? card = await dbContext.MenuCard.Load().ByIdForActivation(
            new MenuCardId(command.Id),
            restaurantId,
            cancellationToken);
        if (card is null)
        {
            throw new InvalidOperationException("The inactive menu card could not be loaded after validation.");
        }

        card.Activate();
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ActivateMenuCardResponse(card.Id.Value);
    }
}
