using FluentValidation;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Authorization;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.UpdateMenuCardsOrder;

public sealed record MenuCardOrderItemDto(Guid MenuCardId, int SortOrder);

public sealed record UpdateMenuCardsOrderRequest(IEnumerable<MenuCardOrderItemDto> MenuCards);

public sealed record UpdateMenuCardsOrderCommand(UpdateMenuCardsOrderRequest Request) : ICommand;

public sealed class UpdateMenuCardsOrderAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<UpdateMenuCardsOrderCommand>
{
    public Task<bool> Authorize(
        UpdateMenuCardsOrderCommand request,
        CancellationToken cancellationToken)
        => Task.FromResult(MenuAuthorization.CanManageRestaurant(
            currentUser,
            companyContext.RestaurantId,
            MenuPermissionConstants.CanManageMenuCards,
            MenuPermissionConstants.CanEditMenuCard));
}

public sealed class UpdateMenuCardsOrderValidator : AbstractValidator<UpdateMenuCardsOrderCommand>
{
    public UpdateMenuCardsOrderValidator(IMenuDbContext dbContext, ICompanyContext companyContext)
    {
        RuleFor(command => command.Request.MenuCards)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Menu card order must not be empty.")
            .Must(items => items.Select(item => item.MenuCardId).Distinct().Count() == items.Count())
            .WithMessage("Menu card IDs must be unique.")
            .Must(items => items
                .Select(item => item.SortOrder)
                .OrderBy(sortOrder => sortOrder)
                .SequenceEqual(Enumerable.Range(0, items.Count())))
            .WithMessage("Menu card sort order must be a contiguous sequence starting at zero.")
            .MustAsync(async (command, items, cancellationToken) =>
            {
                Guid? restaurantId = companyContext.RestaurantId;
                if (!restaurantId.HasValue)
                {
                    return false;
                }

                IReadOnlyCollection<Guid> restaurantCardIds = await dbContext.MenuCard.Query()
                    .IdsByRestaurant(restaurantId.Value, cancellationToken);
                HashSet<Guid> requestedCardIds = items.Select(item => item.MenuCardId).ToHashSet();

                return restaurantCardIds.Count == requestedCardIds.Count
                    && restaurantCardIds.All(requestedCardIds.Contains);
            })
            .WithMessage("Menu card order must contain every card for the selected restaurant.");
    }
}

public sealed class UpdateMenuCardsOrderHandler(
    IMenuDbContext dbContext,
    ICompanyContext companyContext)
    : IRequestHandler<UpdateMenuCardsOrderCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateMenuCardsOrderCommand command,
        CancellationToken cancellationToken)
    {
        Guid restaurantId = companyContext.RestaurantId
            ?? throw new InvalidOperationException("A restaurant context is required for menu card management.");
        IReadOnlyCollection<MenuCard> cards = await dbContext.MenuCard.Load()
            .ByRestaurant(restaurantId, cancellationToken);
        Dictionary<Guid, MenuCard> cardsById = cards.ToDictionary(card => card.Id.Value);

        foreach (MenuCardOrderItemDto item in command.Request.MenuCards)
        {
            if (!cardsById.TryGetValue(item.MenuCardId, out MenuCard? card))
            {
                throw new InvalidOperationException("Validated menu card order contains an unknown menu card.");
            }

            card.UpdateSortOrder(item.SortOrder);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
