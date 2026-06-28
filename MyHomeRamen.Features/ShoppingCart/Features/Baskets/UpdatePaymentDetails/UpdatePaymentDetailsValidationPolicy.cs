using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Common.Contracts.Payments;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Features.ShoppingCart.Features.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdatePaymentDetails;

public sealed class UpdatePaymentDetailsValidationPolicy : AbstractValidator<UpdatePaymentDetailsCommand>
{
    public UpdatePaymentDetailsValidationPolicy(IShoppingCartDbContext dbContext, IPaymentService paymentService)
    {
        RuleFor(x => x.Request.PaymentMethodId)
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("Invalid PaymentMethodId format.");

        When(x => !string.IsNullOrEmpty(x.Request.PaymentChannelId), () =>
        {
            RuleFor(x => x.Request.PaymentChannelId)
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("Invalid PaymentChannelId format.");
        });

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) => await dbContext.ShoppingCarts.GetByIdForUserTracked(cmd.BasketId, cmd.UserId).AnyAsync(ct))
            .WithMessage("Basket not found or not active.")
            .OverridePropertyName(x => x.BasketId);

        RuleFor(x => x.Request)
            .MustAsync(async (req, ct) =>
            {
                Guid channelId = Guid.Empty;

                if (!Guid.TryParse(req.PaymentMethodId, out Guid methodId))
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(req.PaymentChannelId) && !Guid.TryParse(req.PaymentChannelId, out channelId))
                {
                    return false;
                }

                return await paymentService.ValidatePaymentSelectionAsync(methodId, channelId, ct);
            })
            .WithMessage("Invalid payment method or channel selected.");
    }
}

