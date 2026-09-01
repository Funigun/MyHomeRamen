namespace MyHomeRamen.Domain.ShoppingCart.PaymentDetails;

public sealed class PaymentDetails
{
    public string PaymentMethodId { get; private set; } = default!;

    public string PaymentChannelId { get; private set; } = default!;

    private PaymentDetails() { }

    public static PaymentDetails Create(string paymentMethodId, string paymentChannelId)
    {
        return new PaymentDetails
        {
            PaymentMethodId = paymentMethodId,
            PaymentChannelId = paymentChannelId
        };
    }
}
