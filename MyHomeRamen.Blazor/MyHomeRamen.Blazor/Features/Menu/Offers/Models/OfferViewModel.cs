using MyHomeRamen.Blazor.Features.Menu.Offers.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.Offers.Models;

public sealed record OfferViewModel(
    Guid OfferId,
    Guid ProductId,
    string ProductName,
    string Status,
    decimal Price,
    bool CanEdit,
    bool CanRetire)
{
    public static OfferViewModel FromResponse(OfferForManageResponse response) =>
        new(
            response.OfferId,
            response.ProductId,
            response.ProductName,
            response.Status,
            response.Price,
            response.AllowedActions.CanEdit,
            response.AllowedActions.CanRetire);
}

public sealed record OfferProductOptionViewModel(Guid ProductId, string Name)
{
    public static OfferProductOptionViewModel FromResponse(EligibleProductForOfferResponse response) =>
        new(response.ProductId, response.Name);
}
