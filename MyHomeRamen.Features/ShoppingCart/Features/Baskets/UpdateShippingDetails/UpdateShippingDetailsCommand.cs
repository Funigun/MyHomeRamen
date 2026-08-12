using FluentValidation;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdateShippingDetails;

public record UpdateShippingDetailsCommand(BasketId BasketId, UserId UserId, UpdateShippingDetailsRequest Request) : ICommand;

public sealed class UpdateShippingDetailsValidationPolicy : AbstractValidator<UpdateShippingDetailsCommand>
{
    public UpdateShippingDetailsValidationPolicy(IShoppingCartDbContext dbContext)
    {
        RuleFor(x => x.Request)
            .Must(req => req.PersonalPickup || req.Delivery)
            .WithMessage("Either PersonalPickup or Delivery must be selected.");

        RuleFor(x => x.Request)
            .Must(req => !(req.PersonalPickup && req.Delivery))
            .WithMessage("Cannot select both PersonalPickup and Delivery.");

        When(x => x.Request.Delivery, () =>
        {
            RuleFor(x => x.Request.ShippingAddress)
                .NotNull()
                .WithMessage("Shipping address is required for delivery.");

            RuleFor(x => x.Request.ShippingAddress!.Street).NotEmpty().When(x => x.Request.ShippingAddress != null);
            RuleFor(x => x.Request.ShippingAddress!.Building).NotEmpty().When(x => x.Request.ShippingAddress != null);
            RuleFor(x => x.Request.ShippingAddress!.City).NotEmpty().When(x => x.Request.ShippingAddress != null);
            RuleFor(x => x.Request.ShippingAddress!.ZipCode).NotEmpty().When(x => x.Request.ShippingAddress != null);
        });

        When(x => x.Request.PersonalPickup, () =>
        {
            RuleFor(x => x.Request.ShippingAddress)
                .Null()
                .WithMessage("Shipping address must be null for personal pickup.");
        });

        RuleFor(x => x)
            .MustHaveAccessibleBasket(
                dbContext,
                cmd => cmd.BasketId,
                cmd => cmd.UserId);
    }
}

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

