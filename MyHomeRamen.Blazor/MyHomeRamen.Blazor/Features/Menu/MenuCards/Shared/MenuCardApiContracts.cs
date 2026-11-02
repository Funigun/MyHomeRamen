namespace MyHomeRamen.Blazor.Features.Menu.MenuCards.Shared;

public sealed record GetMenuCardOptionsResponse(IEnumerable<MenuCardOptionContract> MenuCards);

public sealed record MenuCardOptionContract(Guid CompanyId, Guid RestaurantId, Guid MenuCardId, string Name);

public sealed record MenuCardResponseContract(
    Guid MenuCardId,
    Guid RestaurantId,
    string Name,
    IEnumerable<MenuSectionContract> Sections);

public sealed record MenuSectionContract(
    Guid Id,
    string Name,
    int SortOrder,
    IEnumerable<MenuOfferContract> Offers);

public sealed record MenuOfferContract(
    Guid OfferId,
    Guid ProductId,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    IEnumerable<MenuCardIngredientContract> Ingredients);

public sealed record MenuCardIngredientContract(Guid Id, string Name, bool IsCustom);

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
