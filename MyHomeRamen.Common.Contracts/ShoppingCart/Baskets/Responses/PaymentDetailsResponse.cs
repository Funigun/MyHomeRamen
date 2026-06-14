namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

public sealed record PaymentDetailsResponse(string PaymentMethodId, string PaymentChannelId);