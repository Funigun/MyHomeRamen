namespace MyHomeRamen.Blazor.Features.Menu.MenuCards.Shared;

public sealed record GetMenuCardOptionsResponse(IEnumerable<MenuCardOptionContract> MenuCards);

public sealed record MenuCardOptionContract(Guid CompanyId, Guid RestaurantId, Guid MenuCardId, string Name);

public sealed record GetMenuCardsForManageResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IEnumerable<MenuCardForManageContract> Items,
    MenuCardAllowedActions AllowedActions);

public sealed record MenuCardForManageContract(
    Guid Id,
    Guid RestaurantId,
    string Name,
    bool IsActive,
    int SortOrder);

public sealed record GetMenuCardForManageResponse(
    Guid MenuCardId,
    Guid RestaurantId,
    string Name,
    bool IsActive,
    int SortOrder,
    MenuCardAllowedActions AllowedActions,
    IEnumerable<MenuSectionForManageContract> Sections,
    IEnumerable<MenuCardOfferChoiceContract> EligibleOffers);

public sealed record MenuCardAllowedActions(bool CanCreate, bool CanEdit, bool CanDelete);

public sealed record MenuSectionForManageContract(
    Guid Id,
    string Name,
    int SortOrder,
    IEnumerable<MenuCardOfferListingContract> Offers);

public sealed record MenuCardOfferListingContract(
    Guid Id,
    Guid OfferId,
    string Name,
    string Description,
    string ImageUrl,
    IEnumerable<string> Categories,
    int SortOrder,
    bool IsAvailable,
    decimal Price,
    string OfferStatus,
    string ProductStatus);

public sealed record MenuCardOfferChoiceContract(
    Guid OfferId,
    string Name,
    string Description,
    string ImageUrl,
    IEnumerable<string> Categories,
    decimal Price,
    bool IsAvailable,
    string OfferStatus,
    string ProductStatus);

public sealed record CreateMenuCardRequest(string Name);

public sealed record UpdateMenuCardRequest(string Name, bool IsActive);

public sealed record MenuCardOrderItemRequest(Guid MenuCardId, int SortOrder);

public sealed record UpdateMenuCardsOrderRequest(IEnumerable<MenuCardOrderItemRequest> MenuCards);

public sealed record MenuCardMutationResponse(Guid MenuCardId);

public sealed record CreateMenuSectionRequest(string Name, int SortOrder);

public sealed record UpdateMenuSectionRequest(string Name, int SortOrder);

public sealed record AddMenuCardOfferRequest(Guid OfferId, int SortOrder);

public sealed record AddMenuCardOffersRequest(IReadOnlyList<Guid> OfferIds);

public sealed record MenuCardOfferAssignmentContract(Guid OfferId, Guid ListingId, int SortOrder);

public sealed record AddMenuCardOffersResponse(IReadOnlyList<MenuCardOfferAssignmentContract> Offers);

public sealed record UpdateMenuCardOfferRequest(int SortOrder);

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
    int SortOrder,
    IEnumerable<MenuCardIngredientContract> Ingredients);

public sealed record MenuCardIngredientContract(Guid Id, string Name, bool IsCustom);
