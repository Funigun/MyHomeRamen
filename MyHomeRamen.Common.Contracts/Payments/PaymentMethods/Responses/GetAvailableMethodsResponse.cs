using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.DTOs;

namespace MyHomeRamen.Common.Contracts.Payments.PaymentMethods.Responses;

public sealed record GetAvailableMethodsResponse(Guid Id, string Name, string Image, IEnumerable<AvailableChannelDto> Channels);
