namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;

public record UpdatePaymentDetailsRequest(string PaymentMethodId, string PaymentChannelId);
