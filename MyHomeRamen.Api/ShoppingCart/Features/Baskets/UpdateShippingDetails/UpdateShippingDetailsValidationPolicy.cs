using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Persistance.Common;
using MyHomeRamen.Persistance.ShoppingCart;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.UpdateShippingDetails;

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
            .MustAsync(async (cmd, ct) => await dbContext.ShoppingCarts.GetByIdForUserTracked(cmd.BasketId, cmd.UserId).AnyAsync(ct))
            .WithMessage("Basket not found or not active.");
    }
}
