namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Models;

public sealed class CheckoutModel
{
    public bool ShoppingCartConfirmed { get; set; }

    public bool ShippingAddressConfirmed { get; set; }

    public bool PaymentMethodConfirmed { get; set; }

    public bool OrderConfirmed { get; set; }
}
