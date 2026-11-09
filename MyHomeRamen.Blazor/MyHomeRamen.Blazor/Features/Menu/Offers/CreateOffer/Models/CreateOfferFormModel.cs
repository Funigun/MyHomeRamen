using MyHomeRamen.Blazor.Features.Menu.Offers.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.Offers.CreateOffer.Models;

public sealed class CreateOfferFormModel
{
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }

    public CreateOfferRequest ToRequest() => new(ProductId, Price);
}
