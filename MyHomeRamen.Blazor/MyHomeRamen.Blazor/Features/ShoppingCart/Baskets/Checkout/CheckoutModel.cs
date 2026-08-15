using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.BasketDetails;
using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.PaymentDetails;
using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.ShippingDetails;
using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Responses;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout;

public sealed class CheckoutModel
{
    public Guid BasketId { get; set; }

    public bool ShoppingCartConfirmed { get; set; }

    public bool ShippingAddressConfirmed { get; set; }

    public bool PaymentMethodConfirmed { get; set; }

    public bool OrderConfirmed { get; set; }

    public ShippingDetailsModel ShippingDetails { get; set; } = new ShippingDetailsModel();

    public PaymentDetailsModel PaymentDetails { get; set; } = new PaymentDetailsModel();

    public ICollection<CheckoutBasketItemModel> Items { get; set; } = [];

    public CheckoutModel FromDetailsResponse(GetCurrentBasketDetailsResponse response)
    {
        BasketId = response.BasketId;

        ShoppingCartConfirmed = false;
        ShippingAddressConfirmed = false;
        PaymentMethodConfirmed = false;
        OrderConfirmed = false;

        Items = response.Items.Select(CheckoutBasketItemModel.FromDetailsDto).ToList();

        return this;
    }

    public void SetShippingDetails(ShippingDetailsResponse shippingDetails)
    {
        ShippingDetails = ShippingDetailsModel.FromResponse(shippingDetails);
    }

    public void SetPaymentDetails(PaymentDetailsResponse paymentDetails)
    {
        PaymentDetails = PaymentDetailsModel.FromResponse(paymentDetails);
    }
}
