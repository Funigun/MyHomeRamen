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

namespace MyHomeRamen.Features.Menu.Features.MenuCards.CreateMenuSection;

public sealed record CreateMenuSectionCommand(Guid MenuCardId, string Name, int SortOrder)
    : ICommand<CreateMenuSectionResponse>;

public sealed class CreateMenuSectionAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<CreateMenuSectionCommand>
{
    public Task<bool> Authorize(CreateMenuSectionCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanAddMenuCard));
}

public sealed class CreateMenuSectionValidator : AbstractValidator<CreateMenuSectionCommand>
{
    public CreateMenuSectionValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command.MenuCardId)
            .NotEmpty()
            .WithMessage("Menu card ID must not be empty.")
            .MustAsync(async (id, cancellationToken) =>
                await dbContext.MenuCard.Exists(card => card.Id == new MenuCardId(id), cancellationToken))
            .WithMessage("A menu card at this restaurant must exist.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Menu section name must not be empty.")
            .MaximumLength(MenuSectionConstants.MaxNameLength)
            .WithMessage($"Menu section name must not exceed {MenuSectionConstants.MaxNameLength} characters.");

        RuleFor(command => command.SortOrder)
            .GreaterThanOrEqualTo(MenuSectionConstants.MinSortOrder)
            .WithMessage("Menu section sort order must not be negative.");
    }
}

public sealed class CreateMenuSectionHandler(IMenuDbContext dbContext)
    : IRequestHandler<CreateMenuSectionCommand, CreateMenuSectionResponse>
{
    public async Task<CreateMenuSectionResponse> Handle(
        CreateMenuSectionCommand command,
        CancellationToken cancellationToken)
    {
        MenuCard? card = await dbContext.MenuCard.Load()
            .ById(new MenuCardId(command.MenuCardId), cancellationToken);
        if (card is null)
        {
            throw new InvalidOperationException("The menu card could not be loaded after validation.");
        }

        MenuSection section = card.AddSection(
            new MenuSectionId(Guid.CreateVersion7()),
            command.Name,
            command.SortOrder);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateMenuSectionResponse(section.Id.Value);
    }
}
