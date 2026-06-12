using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Domain.Payments.PaymentMethods;

namespace MyHomeRamen.Api.Payments.Features.PaymentMethods.GetAvailableMethods;

internal static class Mappings
{
    internal static PaymentMethodResponse ToResponse(this PaymentMethod paymentMethod)
    {
        return new
        (
            paymentMethod.Id,
            paymentMethod.Name,
            paymentMethod.ImageUrl,
            paymentMethod.PaymentChannels.Select(ToResponses)
        );
    }

    private static ChannelDto ToResponses(PaymentChannel channel)
    {
        return new
        (
            channel.Id,
            channel.Name,
            channel.ImageUrl
        );
    }
}
