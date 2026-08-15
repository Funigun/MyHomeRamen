using MyHomeRamen.Blazor.Features.Payments.Common.Services.Contracts.PaymentMethods.Responses;

namespace MyHomeRamen.Blazor.Features.Payments.PaymentMethods.Models;

public sealed class AvailableMethodModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public List<AvailableChannelModel> Channels { get; set; } = [];

    public static AvailableMethodModel FromResponse(GetAvailableMethodsResponse response) => new()
    {
        Id = response.Id,
        Name = response.Name,
        Image = response.Image,
        Channels = response.Channels.Select(AvailableChannelModel.FromDto).ToList()
    };
}
