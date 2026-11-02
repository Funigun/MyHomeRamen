using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.CreateMenuCard;

public sealed record CreateMenuCardCommand(string Name) : ICommand<CreateMenuCardResponse>;

public sealed class CreateMenuCardAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<CreateMenuCardCommand>
{
    public Task<bool> Authorize(CreateMenuCardCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanAddMenuCard));
}

public sealed class CreateMenuCardValidator : AbstractValidator<CreateMenuCardCommand>
{
    public CreateMenuCardValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Menu card name must not be empty.")
            .MaximumLength(MenuCardConstants.MaxNameLength)
            .WithMessage($"Menu card name must not exceed {MenuCardConstants.MaxNameLength} characters.");
    }
}

public sealed class CreateMenuCardHandler(IMenuDbContext dbContext)
    : IRequestHandler<CreateMenuCardCommand, CreateMenuCardResponse>
{
    public async Task<CreateMenuCardResponse> Handle(
        CreateMenuCardCommand command,
        CancellationToken cancellationToken)
    {
        MenuCard card = MenuCard.Create(
            new MenuCardId(Guid.CreateVersion7()),
            dbContext.RestaurantId,
            command.Name);
        dbContext.MenuCard.Add(card);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateMenuCardResponse(card.Id.Value);
    }
}
