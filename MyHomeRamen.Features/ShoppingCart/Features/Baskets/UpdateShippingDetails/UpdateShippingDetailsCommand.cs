using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdateShippingDetails;

public record UpdateShippingDetailsCommand(BasketId BasketId, UserId UserId, UpdateShippingDetailsRequest Request) : ICommand;

public sealed class UpdateShippingDetailsHandler(IShoppingCartDbContext dbContext) : ICommandHandler<UpdateShippingDetailsCommand>
{
    public async Task Handle(UpdateShippingDetailsCommand request, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.Basket.Specification()
                                       .GetByIdForUserWithShippingTrackedAsync(request.BasketId, request.UserId, cancellationToken)
                                       ?? throw new InvalidOperationException("Basket was not found.");

        basket.UpdateShippingDetails(request.Request.ToDomain());

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

