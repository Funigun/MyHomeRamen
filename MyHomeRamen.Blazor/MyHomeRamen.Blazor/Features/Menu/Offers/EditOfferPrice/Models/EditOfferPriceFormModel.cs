using MyHomeRamen.Blazor.Features.Menu.Offers.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.Offers.EditOfferPrice.Models;

public sealed class EditOfferPriceFormModel
{
    public decimal Price { get; set; }

    public UpdateOfferPriceRequest ToRequest() => new(Price);
}
