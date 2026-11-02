using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.UpdateMenuCard;

public sealed record UpdateMenuCardCommand(Guid Id, string Name, bool IsActive)
    : ICommand<UpdateMenuCardResponse>;

public sealed class UpdateMenuCardAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<UpdateMenuCardCommand>
{
    public Task<bool> Authorize(UpdateMenuCardCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanEditMenuCard));
}

public sealed class UpdateMenuCardValidator : AbstractValidator<UpdateMenuCardCommand>
{
    public UpdateMenuCardValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Menu card name must not be empty.")
            .MaximumLength(MenuCardConstants.MaxNameLength)
            .WithMessage($"Menu card name must not exceed {MenuCardConstants.MaxNameLength} characters.");

        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Menu card ID must not be empty.")
            .MustAsync(async (id, cancellationToken) =>
                await dbContext.MenuCard.Exists(card => card.Id == new MenuCardId(id), cancellationToken))
            .WithMessage("A menu card at this restaurant must exist.");
    }
}

public sealed class UpdateMenuCardHandler(IMenuDbContext dbContext)
    : IRequestHandler<UpdateMenuCardCommand, UpdateMenuCardResponse>
{
    public async Task<UpdateMenuCardResponse> Handle(
        UpdateMenuCardCommand command,
        CancellationToken cancellationToken)
    {
        MenuCard? card = await dbContext.MenuCard.Load()
            .ById(new MenuCardId(command.Id), cancellationToken);
        if (card is null)
        {
            throw new InvalidOperationException("The menu card could not be loaded after validation.");
        }

        card.UpdateName(command.Name);
        card.SetActive(command.IsActive);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMenuCardResponse(card.Id.Value);
    }
}
