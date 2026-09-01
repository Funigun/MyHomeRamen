using FluentValidation;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.ClearBasket;

public sealed record ClearBasketCommand(BasketId BasketId, UserId UserId) : ICommand;

public sealed class ClearBasketAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<ClearBasketCommand>
{
    public async Task<bool> Authorize(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(currentUser.CanRemoveProduct());
    }
}

public sealed class ClearBasketValidationPolicy : AbstractValidator<ClearBasketCommand>
{
    public ClearBasketValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x.BasketId)
            .MustBeAccessibleBasket(
                dbContext,
                command => command.UserId);
    }
}

public sealed class ClearBasketHandler(IShoppingCartDbContext dbContext) : ICommandHandler<ClearBasketCommand>
{
    public async Task Handle(ClearBasketCommand command, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.Basket.Load()
            .GetByIdForUserTrackedAsync(command.BasketId, command.UserId, cancellationToken);

        basket.Clear();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
