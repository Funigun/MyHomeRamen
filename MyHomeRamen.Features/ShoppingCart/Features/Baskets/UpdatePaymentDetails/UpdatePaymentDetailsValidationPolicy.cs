using FluentValidation;
using MyHomeRamen.Common.Contracts.Payments;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

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
            .MustHaveAccessibleBasket(
                dbContext,
                cmd => cmd.BasketId,
                cmd => cmd.UserId,
                nameof(UpdatePaymentDetailsCommand.BasketId));

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

