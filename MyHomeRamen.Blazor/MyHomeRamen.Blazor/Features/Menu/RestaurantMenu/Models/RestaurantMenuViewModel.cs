using MyHomeRamen.Blazor.Features.Menu.MenuCards.Shared;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.DTOs;

namespace MyHomeRamen.Blazor.Features.Menu.RestaurantMenu.Models;

public sealed record RestaurantMenuCardOptionViewModel(Guid MenuCardId, Guid RestaurantId, string Name);

public sealed record RestaurantMenuCustomizationViewModel(
    Guid OfferId,
    Guid ProductId,
    string Name,
    string Description,
    IReadOnlyCollection<IngredientDto> BaseIngredients,
    IReadOnlyCollection<IngredientDto> AvailableExtras)
{
    public static RestaurantMenuCustomizationViewModel FromContract(GetOfferCustomizationResponse response)
        => new(
            response.OfferId,
            response.ProductId,
            response.Name,
            response.Description,
            response.BaseIngredients.Select(ingredient =>
                new IngredientDto(ingredient.Id, ingredient.Name, string.Empty, ingredient.Price)).ToArray(),
            response.AvailableExtras.Select(ingredient =>
                new IngredientDto(ingredient.Id, ingredient.Name, string.Empty, ingredient.Price)).ToArray());
}

public sealed record RestaurantMenuViewModel(
    Guid MenuCardId,
    Guid RestaurantId,
    string Name,
    IReadOnlyCollection<RestaurantMenuSectionViewModel> Sections)
{
    public static RestaurantMenuViewModel FromContract(MenuCardResponseContract response)
        => new(
            response.MenuCardId,
            response.RestaurantId,
            response.Name,
            response.Sections
                .OrderBy(section => section.SortOrder)
                .Select(section => new RestaurantMenuSectionViewModel(
                    section.Id,
                    section.Name,
                    section.Offers
                        .Select(offer => new RestaurantMenuOfferViewModel(
                            offer.OfferId,
                            offer.ProductId,
                            offer.Name,
                            offer.Description,
                            offer.Price,
                            offer.ImageUrl,
                            string.Join(", ", offer.Ingredients
                                .Where(ingredient => !ingredient.IsCustom)
                                .Select(ingredient => ingredient.Name))))
                        .ToArray()))
                .ToArray());
}

public sealed record RestaurantMenuSectionViewModel(
    Guid Id,
    string Name,
    IReadOnlyCollection<RestaurantMenuOfferViewModel> Offers);

public sealed record RestaurantMenuOfferViewModel(
    Guid OfferId,
    Guid ProductId,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    string BaseIngredients);
