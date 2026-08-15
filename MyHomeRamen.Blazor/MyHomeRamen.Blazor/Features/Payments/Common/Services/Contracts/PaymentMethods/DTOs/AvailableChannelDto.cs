namespace MyHomeRamen.Blazor.Features.Payments.Common.Services.Contracts.PaymentMethods.DTOs;

public sealed record AvailableChannelDto(Guid Id, string Name, string ImageUrl);
