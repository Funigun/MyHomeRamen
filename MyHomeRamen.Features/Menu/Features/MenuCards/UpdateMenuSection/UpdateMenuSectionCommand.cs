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

namespace MyHomeRamen.Features.Menu.Features.MenuCards.UpdateMenuSection;

public sealed record UpdateMenuSectionCommand(Guid MenuCardId, Guid SectionId, string Name, int SortOrder)
    : ICommand<UpdateMenuSectionResponse>;

public sealed class UpdateMenuSectionAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<UpdateMenuSectionCommand>
{
    public Task<bool> Authorize(UpdateMenuSectionCommand request, CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanEditMenuCard));
}

public sealed class UpdateMenuSectionValidator : AbstractValidator<UpdateMenuSectionCommand>
{
    public UpdateMenuSectionValidator(IMenuDbContext dbContext)
    {
        RuleFor(command => command)
            .MustAsync(async (command, cancellationToken) =>
            {
                MenuCard? card = await dbContext.MenuCard.Load()
                    .ById(new MenuCardId(command.MenuCardId), cancellationToken);
                return card?.Sections.Any(section => section.Id == new MenuSectionId(command.SectionId)) == true;
            })
            .WithMessage("The menu section must belong to a menu card at this restaurant.");

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

public sealed class UpdateMenuSectionHandler(IMenuDbContext dbContext)
    : IRequestHandler<UpdateMenuSectionCommand, UpdateMenuSectionResponse>
{
    public async Task<UpdateMenuSectionResponse> Handle(
        UpdateMenuSectionCommand command,
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

        section.Update(command.Name, command.SortOrder);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMenuSectionResponse(section.Id.Value);
    }
}
