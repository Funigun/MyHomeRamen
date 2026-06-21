using MyHomeRamen.Common.Contracts.Payments.PaymentMethods.DTOs;

namespace MyHomeRamen.Blazor.Features.Payments.PaymentMethods.Models;

public sealed class AvailableChannelModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public static AvailableChannelModel FromDto(AvailableChannelDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        ImageUrl = dto.ImageUrl
    };
}
