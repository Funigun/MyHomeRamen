using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.PaymentDetails;

public class PaymentDetailsModel
{
    public string PaymentMethodId { get; set; }

    public string PaymentChannelId { get; set; }

    public static PaymentDetailsModel FromResponse(PaymentDetailsResponse paymentDetailsResponse)
    {
        return new PaymentDetailsModel
        {
            PaymentMethodId = paymentDetailsResponse.PaymentMethodId,
            PaymentChannelId = paymentDetailsResponse.PaymentChannelId
        };
    }
}
