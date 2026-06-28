using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;
using MyHomeRamen.Features.ShoppingCart.Features.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdatePaymentDetails;

public sealed class UpdatePaymentDetailsHandler(IShoppingCartDbContext dbContext) : ICommandHandler<UpdatePaymentDetailsCommand>
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

