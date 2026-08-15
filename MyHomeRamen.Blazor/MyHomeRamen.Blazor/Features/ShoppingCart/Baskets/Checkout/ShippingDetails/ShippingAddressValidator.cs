using MyHomeRamen.Blazor.Common.Models;
using MyHomeRamen.Blazor.Features.Account.Components.Validators;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.ShippingDetails;

public sealed class ShippingAddressValidator : BaseValidator<ShippingAddressModel>
{
    public ShippingAddressValidator()
    {
        RuleFor(x => x.Street).ValidStreet();
        RuleFor(x => x.Building).ValidBuilding();
        RuleFor(x => x.Apartment).ValidApartment();
        RuleFor(x => x.City).ValidCity();
        RuleFor(x => x.ZipCode).ValidZipCode();
    }
}
