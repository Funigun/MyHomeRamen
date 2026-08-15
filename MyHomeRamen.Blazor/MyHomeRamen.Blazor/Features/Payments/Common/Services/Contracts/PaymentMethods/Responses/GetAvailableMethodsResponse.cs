using MyHomeRamen.Blazor.Features.Payments.Common.Services.Contracts.PaymentMethods.DTOs;

namespace MyHomeRamen.Blazor.Features.Payments.Common.Services.Contracts.PaymentMethods.Responses;

public sealed record GetAvailableMethodsResponse(Guid Id, string Name, string Image, IEnumerable<AvailableChannelDto> Channels);
