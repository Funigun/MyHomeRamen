using MyHomeRamen.Blazor.Features.Menu.MenuCards.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.MenuCards.Models;

public sealed record MenuCardViewModel(
    Guid MenuCardId,
    Guid RestaurantId,
    string Name,
    bool IsActive,
    int SortOrder,
    bool CanEdit,
    bool CanDelete)
{
    public static MenuCardViewModel FromResponse(
        MenuCardForManageContract response,
        MenuCardAllowedActions allowedActions) =>
        new(
            response.Id,
            response.RestaurantId,
            response.Name,
            response.IsActive,
            response.SortOrder,
            allowedActions.CanEdit,
            allowedActions.CanDelete);
}

public sealed record MenuCardDetailsViewModel(
    Guid MenuCardId,
    Guid RestaurantId,
    string Name,
    bool IsActive,
    int SortOrder,
    MenuCardAllowedActions AllowedActions,
    IReadOnlyList<MenuCardSectionViewModel> Sections,
    IReadOnlyList<MenuCardOfferChoiceViewModel> EligibleOffers)
{
    public static MenuCardDetailsViewModel FromResponse(GetMenuCardForManageResponse response) =>
        new(
            response.MenuCardId,
            response.RestaurantId,
            response.Name,
            response.IsActive,
            response.SortOrder,
            response.AllowedActions,
            response.Sections
                .OrderBy(section => section.SortOrder)
                .Select(section => new MenuCardSectionViewModel(
                    section.Id,
                    section.Name,
                    section.SortOrder,
                    section.Offers
                        .OrderBy(offer => offer.SortOrder)
                        .Select(offer => new MenuCardListingViewModel(
                            offer.Id,
                            offer.OfferId,
                            offer.Name,
                            offer.Description,
                            offer.ImageUrl,
                            offer.Categories.ToArray(),
                            offer.SortOrder,
                            offer.IsAvailable,
                            offer.Price,
                            offer.OfferStatus,
                            offer.ProductStatus))
                        .ToList()))
                .ToList(),
            response.EligibleOffers
                .OrderBy(offer => offer.Name, StringComparer.CurrentCulture)
                .Select(offer => new MenuCardOfferChoiceViewModel(
                    offer.OfferId,
                    offer.Name,
                    offer.Description,
                    offer.ImageUrl,
                    offer.Categories.ToArray(),
                    offer.Price,
                    offer.IsAvailable,
                    offer.OfferStatus,
                    offer.ProductStatus))
                .ToList());
}

public sealed class MenuCardSectionViewModel(
    Guid id,
    string name,
    int sortOrder,
    IReadOnlyList<MenuCardListingViewModel> offers)
{
    public Guid Id { get; } = id;
    public string Name { get; set; } = name;
    public int SortOrder { get; set; } = sortOrder;
    public IReadOnlyList<MenuCardListingViewModel> Offers { get; } = offers;
}

public sealed class MenuCardListingViewModel(
    Guid id,
    Guid offerId,
    string name,
    string description,
    string imageUrl,
    IReadOnlyList<string> categories,
    int sortOrder,
    bool isAvailable,
    decimal price,
    string offerStatus,
    string productStatus)
{
    public Guid Id { get; } = id;
    public Guid OfferId { get; } = offerId;
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string ImageUrl { get; } = imageUrl;
    public IReadOnlyList<string> Categories { get; } = categories;
    public int SortOrder { get; set; } = sortOrder;
    public bool IsAvailable { get; } = isAvailable;
    public decimal Price { get; } = price;
    public string OfferStatus { get; } = offerStatus;
    public string ProductStatus { get; } = productStatus;
}

public sealed record MenuCardOfferChoiceViewModel(
    Guid OfferId,
    string Name,
    string Description,
    string ImageUrl,
    IReadOnlyList<string> Categories,
    decimal Price,
    bool IsAvailable,
    string OfferStatus,
    string ProductStatus);
