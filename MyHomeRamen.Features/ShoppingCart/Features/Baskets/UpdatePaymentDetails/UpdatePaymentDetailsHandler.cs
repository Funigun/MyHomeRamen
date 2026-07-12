using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdatePaymentDetails;

public sealed class UpdatePaymentDetailsHandler(IShoppingCartDbContext dbContext) : ICommandHandler<UpdatePaymentDetailsCommand>
{
    public async Task Handle(UpdatePaymentDetailsCommand request, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.Basket.Specification()
                                        .GetByIdForUserWithPaymentTrackedAsync(request.BasketId, request.UserId, cancellationToken)
                                        ?? throw new InvalidOperationException("Basket was not found.");

        PaymentDetails details = PaymentDetails.Create(request.Request.PaymentMethodId, request.Request.PaymentChannelId);
        basket.UpdatePaymentDetails(details);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

