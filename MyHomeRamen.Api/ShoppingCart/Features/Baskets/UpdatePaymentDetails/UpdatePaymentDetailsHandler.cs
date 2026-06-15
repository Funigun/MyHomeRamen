using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.UpdatePaymentDetails;

internal sealed class UpdatePaymentDetailsHandler(IShoppingCartDbContext dbContext) : ICommandHandler<UpdatePaymentDetailsCommand>
{
    public async Task Handle(UpdatePaymentDetailsCommand request, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.ShoppingCarts
                                        .GetByIdForUserWithPaymentTracked(request.BasketId, request.UserId)
                                        .FirstAsync(cancellationToken);

        PaymentDetails details = PaymentDetails.Create(request.Request.PaymentMethodId, request.Request.PaymentChannelId);
        basket.UpdatePaymentDetails(details);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
