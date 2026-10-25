using FluentValidation;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdateBasketItemQuantity;

public sealed record UpdateBasketItemQuantityRequest(int Quantity);

public sealed record UpdateBasketItemQuantityCommand(
    BasketId BasketId,
    BasketItemId BasketItemId,
    UserId UserId,
    UpdateBasketItemQuantityRequest Request) : ICommand;

public sealed class UpdateBasketItemQuantityAuthorizationPolicy(ICurrentUser currentUser)
    : IAuthorizationPolicy<UpdateBasketItemQuantityCommand>
{
    public async Task<bool> Authorize(UpdateBasketItemQuantityCommand request, CancellationToken cancellationToken)
        => await Task.FromResult(currentUser.CanManageBasket());
}

public sealed class UpdateBasketItemQuantityValidationPolicy : AbstractValidator<UpdateBasketItemQuantityCommand>
{
    public UpdateBasketItemQuantityValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x.Request.Quantity).MustBeValidBasketItemQuantity();
        RuleFor(x => x.BasketId)
            .MustBeAccessibleBasket(dbContext, command => command.UserId);
        RuleFor(x => x.BasketItemId).NotEmpty();
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                Basket? basket = await dbContext.Basket.Load()
                    .GetByIdForUserTrackedAsync(command.BasketId, command.UserId, cancellationToken);
                return basket?.Items.Any(item => item.Id == command.BasketItemId) ?? false;
            })
            .WithMessage("Basket item was not found in the specified basket.");
    }
}

public sealed class UpdateBasketItemQuantityHandler(IShoppingCartDbContext dbContext)
    : IRequestHandler<UpdateBasketItemQuantityCommand, Unit>
{
    public async Task<Unit> Handle(UpdateBasketItemQuantityCommand command, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.Basket.Load()
            .GetByIdForUserTrackedAsync(command.BasketId, command.UserId, cancellationToken)
            ?? throw new InvalidOperationException("Basket was not found.");

        basket.Items.Single(item => item.Id == command.BasketItemId)
            .UpdateQuantity(command.Request.Quantity);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
