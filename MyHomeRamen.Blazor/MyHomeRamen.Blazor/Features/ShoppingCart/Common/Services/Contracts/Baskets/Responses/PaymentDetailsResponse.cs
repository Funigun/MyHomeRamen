namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Responses;

public sealed record PaymentDetailsResponse(string PaymentMethodId, string PaymentChannelId);