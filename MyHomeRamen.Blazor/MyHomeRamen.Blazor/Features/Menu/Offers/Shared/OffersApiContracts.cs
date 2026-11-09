namespace MyHomeRamen.Blazor.Features.Menu.Offers.Shared;

public static class OfferStatusCodes
{
    public const string Active = "Active";
    public const string Retired = "Retired";
}

public sealed record OfferAllowedActions(bool CanCreate, bool CanEdit, bool CanRetire);

public sealed record GetOffersForManageResponse(
    IEnumerable<OfferForManageResponse> Offers,
    IEnumerable<EligibleProductForOfferResponse> EligibleProducts,
    OfferAllowedActions AllowedActions);

public sealed record OfferForManageResponse(
    Guid OfferId,
    Guid ProductId,
    string ProductName,
    string Status,
    decimal Price,
    OfferAllowedActions AllowedActions);

public sealed record EligibleProductForOfferResponse(Guid ProductId, string Name);

public sealed record CreateOfferRequest(Guid ProductId, decimal Price);

public sealed record UpdateOfferPriceRequest(decimal Price);

public sealed record OfferMutationResponse(Guid OfferId);

public sealed record OfferIngredientContract(Guid Id, string Name, decimal Price);

public sealed record GetOfferCustomizationResponse(
    Guid OfferId,
    Guid ProductId,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    IEnumerable<OfferIngredientContract> BaseIngredients,
    IEnumerable<OfferIngredientContract> AvailableExtras);
