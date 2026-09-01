using FluentValidation;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Payments.ExternalApi;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdatePaymentDetails;

public record UpdatePaymentDetailsCommand(BasketId BasketId, UserId UserId, UpdatePaymentDetailsRequest Request) : ICommand;

public sealed class UpdatePaymentDetailsAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<UpdatePaymentDetailsCommand>
{
    public async Task<bool> Authorize(UpdatePaymentDetailsCommand request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(currentUser.CanCheckout());
    }
}

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

                bool isMethodIdValid = Guid.TryParse(req.PaymentMethodId, out Guid methodId);
                bool isChannelIdValid = string.IsNullOrEmpty(req.PaymentChannelId) || Guid.TryParse(req.PaymentChannelId, out channelId);

                return isMethodIdValid && isChannelIdValid && await paymentService.ValidatePaymentSelectionAsync(methodId, channelId, ct);
            })
            .WithMessage("Invalid payment method or channel selected.");
    }
}

public sealed class UpdatePaymentDetailsHandler(IShoppingCartDbContext dbContext) : ICommandHandler<UpdatePaymentDetailsCommand>
{
    public async Task Handle(UpdatePaymentDetailsCommand request, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.Basket.Load()
                                        .GetByIdForUserWithPaymentTrackedAsync(request.BasketId, request.UserId, cancellationToken)
                                        ?? throw new InvalidOperationException("Basket was not found.");

        PaymentDetails details = PaymentDetails.Create(request.Request.PaymentMethodId, request.Request.PaymentChannelId);
        basket.UpdatePaymentDetails(details);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
