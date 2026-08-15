namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Requests;

public record UpdatePaymentDetailsRequest(string PaymentMethodId, string PaymentChannelId);
