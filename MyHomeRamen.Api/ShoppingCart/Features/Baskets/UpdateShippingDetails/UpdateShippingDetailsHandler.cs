using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Persistance.Common;


namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.UpdateShippingDetails;

internal sealed class UpdateShippingDetailsHandler(IShoppingCartDbContext dbContext) : ICommandHandler<UpdateShippingDetailsCommand>
{
    public async Task Handle(UpdateShippingDetailsCommand request, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.ShoppingCarts
                                       .GetByIdForUserWithShippingTracked(request.BasketId, request.UserId)
                                       .FirstAsync(cancellationToken);

        basket.UpdateShippingDetails(request.Request.ToDomain());

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
