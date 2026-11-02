using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.MenuSectionItems;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.MenuCards.Common;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCard;

public sealed record MenuCardIngredientDto(Guid Id, string Name, bool IsCustom);

public sealed record MenuOfferDto(
    Guid OfferId,
    Guid ProductId,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    IEnumerable<MenuCardIngredientDto> Ingredients);

public sealed record MenuSectionDto(
    Guid Id,
    string Name,
    int SortOrder,
    IEnumerable<MenuOfferDto> Offers);

public sealed record GetMenuCardQuery(Guid Id) : IQuery<MenuCardResponse?>;

public sealed class GetMenuCardAuthorizationPolicy(ICurrentUser currentUser, ICompanyContext companyContext)
    : IAuthorizationPolicy<GetMenuCardQuery>
{
    public Task<bool> Authorize(GetMenuCardQuery request, CancellationToken cancellationToken)
        => Task.FromResult(
            companyContext.RestaurantId.HasValue
            && currentUser.HasPermission(
                "Menu",
                MenuPermissionConstants.CanViewMenuCard,
                companyContext.RestaurantId));
}

public sealed class GetMenuCardHandler(IMenuCardRepository menuCardRepository)
    : IRequestHandler<GetMenuCardQuery, MenuCardResponse?>
{
    public async Task<MenuCardResponse?> Handle(
        GetMenuCardQuery query,
        CancellationToken cancellationToken)
    {
        MenuCard? card = await menuCardRepository.Query()
            .ActiveById(new MenuCardId(query.Id), cancellationToken);
        if (card is null)
        {
            return null;
        }

        IEnumerable<MenuSectionDto> sections = card.Sections
            .OrderBy(section => section.SortOrder)
            .Select(section => new MenuSectionDto(
                section.Id.Value,
                section.Name,
                section.SortOrder,
                section.Items
                    .OrderBy(item => item.SortOrder)
                    .Select(MapOffer)));

        return new MenuCardResponse(card.Id.Value, card.RestaurantId, card.Name, sections);
    }

    private static MenuOfferDto MapOffer(MenuSectionItem item)
    {
        RestaurantProductOffer offer = item.RestaurantProductOffer;
        Product product = offer.Product;
        IEnumerable<MenuCardIngredientDto> ingredients = product.BaseIngredients
            .Select(ingredient => new MenuCardIngredientDto(ingredient.Id.Value, ingredient.Name, false))
            .Concat(product.CustomIngredients.Select(
                ingredient => new MenuCardIngredientDto(ingredient.Id.Value, ingredient.Name, true)));

        return new MenuOfferDto(
            offer.Id.Value,
            product.Id.Value,
            product.Name,
            product.Description,
            offer.Price,
            product.ImageUrl,
            ingredients);
    }
}
