using FluentValidation;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.DeleteBasketItem;

public sealed record DeleteBasketItemCommand(BasketId BasketId, BasketItemId BasketItemId) : ICommand;

public sealed class DeleteBasketItemValidationPolicy : AbstractValidator<DeleteBasketItemCommand>
{
    public DeleteBasketItemValidationPolicy(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    {
        RuleFor(x => x.BasketId)
            .MustBeAccessibleBasket(
                dbContext,
                _ => new UserId(currentUser.UserId));

        RuleFor(x => x.BasketItemId)
            .NotEmpty()
            .MustAsync(async (command, basketItemId, ct) =>
            {
                UserId userId = new(currentUser.UserId);
                Basket? basket = await dbContext.Basket.Load().GetByIdForUserTrackedAsync(command.BasketId, userId, ct);
                return basket?.Items.Any(item => item.Id == basketItemId) ?? false;
            })
            .WithMessage("Basket item was not found in the specified basket.");
    }
}

public sealed class DeleteBasketItemHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    : ICommandHandler<DeleteBasketItemCommand>
{
    public async Task Handle(DeleteBasketItemCommand command, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        Basket basket = await dbContext.Basket.Load()
            .GetByIdForUserTrackedAsync(command.BasketId, userId, cancellationToken)
            ?? throw new InvalidOperationException("Basket was not found.");

        basket.RemoveItem(command.BasketItemId);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

