namespace MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;

public sealed record PaymentMethodResponse(Guid Id, string Name, string Image, IEnumerable<ChannelDto> Channels);
