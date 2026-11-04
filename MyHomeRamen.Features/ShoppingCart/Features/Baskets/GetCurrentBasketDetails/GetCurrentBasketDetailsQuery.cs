using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;

public sealed record GetCurrentBasketDetailsQuery : IQuery<GetCurrentBasketDetailsResponse?>;

public sealed class GetCurrentBasketDetailsAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetCurrentBasketDetailsQuery>
{
    public async Task<bool> Authorize(GetCurrentBasketDetailsQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(currentUser.CanManageBasket());
    }
}

public sealed record GetCurrentBasketDetailsQueryOptions(UserId UserId, Guid? RestaurantId)
    : DbQueryOptions<Basket, CurrentBasketDetailsDto>
    (
        new()
        {
            Filter = basket => basket.UserId == UserId
                && (!RestaurantId.HasValue || basket.RestaurantId == RestaurantId)
                && basket.Status == BasketStatus.Active,
            Selector = basket => new CurrentBasketDetailsDto(
                basket.Id.Value,
                basket.RestaurantId,
                basket.Items.Select(item => new BasketDetailsItemDto(
                    item.Id.Value,
                    basket.RestaurantId,
                    item.RestaurantProductOfferId,
                    item.Quantity,
                    item.Price,
                    null,
                    false,
                    false,
                    item.Comment,
                    new BasketDetailsItemProductDto(
                        item.Product.Id.Value,
                        item.Product.OriginalId.Value,
                        item.Product.Name,
                        item.Product.Description,
                        item.Product.ImageUrl,
                        item.Product.BaseIngredients.Select(ingredient => new BasketDetailsIngredientDto(ingredient.Id.Value, ingredient.OriginalId.Value, ingredient.Quantity, ingredient.Name, ingredient.Description, ingredient.Price)),
                        item.Product.CustomIngredients.Select(ingredient => new BasketDetailsIngredientDto(ingredient.Id.Value, ingredient.OriginalId.Value, ingredient.Quantity, ingredient.Name, ingredient.Description, ingredient.Price))))))
        }
    );

public sealed class GetCurrentBasketDetailsHandler(
    IShoppingCartDbContext dbContext,
    ICurrentUser currentUser,
    IMenuService menuService,
    ICompanyContext companyContext)
    : IRequestHandler<GetCurrentBasketDetailsQuery, GetCurrentBasketDetailsResponse?>
{
    public async Task<GetCurrentBasketDetailsResponse?> Handle(GetCurrentBasketDetailsQuery request, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        CurrentBasketDetailsDto? basket = await dbContext.Basket.Query().GetCurrentBasketDetailsAsync(
            new GetCurrentBasketDetailsQueryOptions(userId, companyContext.RestaurantId),
            cancellationToken);

        if (basket is null)
        {
            return null;
        }

        List<BasketDetailsItemDto> items = [];
        foreach (BasketDetailsItemDto item in basket.Items)
        {
            MenuOfferResult? offer = await menuService.GetOfferForBasketAsync(item.RestaurantId, item.OfferId, cancellationToken);
            Dictionary<Guid, decimal> availableExtras = offer?.AvailableExtras.ToDictionary(extra => extra.Id, extra => extra.Price) ?? [];
            bool isAvailable = offer is not null
                && item.Product.CustomIngredients.All(ingredient => availableExtras.ContainsKey(ingredient.OriginalId));
            decimal? currentPrice = isAvailable
                ? (offer!.Price + item.Product.CustomIngredients.Sum(ingredient =>
                    availableExtras[ingredient.OriginalId] * ingredient.Quantity)) * item.Quantity
                : null;

            items.Add(item with
            {
                CurrentPrice = currentPrice,
                IsPriceChanged = currentPrice.HasValue && currentPrice.Value != item.Price,
                IsAvailable = isAvailable
            });
        }

        return new GetCurrentBasketDetailsResponse(
            basket.BasketId,
            basket.RestaurantId,
            items,
            currentUser.BasketAllowedActions());
    }
}

internal static class Mappings
{
    public static GetCurrentBasketDetailsResponse ToResponse(this Basket basket, ICurrentUser currentUser)
        => new(
            basket.Id.Value,
            basket.RestaurantId,
            basket.Items.Select(i => i.ToDto(basket.RestaurantId)),
            currentUser.BasketAllowedActions());

    private static BasketDetailsItemDto ToDto(this BasketItem item, Guid restaurantId)
        => new(
            item.Id.Value,
            restaurantId,
            item.RestaurantProductOfferId,
            item.Quantity,
            item.Price,
            null,
            false,
            false,
            item.Comment,
            item.Product.ToProductDto());

    private static BasketDetailsItemProductDto ToProductDto(this Product product)
        => new(
            product.Id.Value,
            product.OriginalId.Value,
            product.Name,
            product.Description,
            product.ImageUrl,
            product.BaseIngredients.Select(i => new BasketDetailsIngredientDto(i.Id.Value, i.OriginalId.Value, i.Quantity, i.Name, i.Description, i.Price)),
            product.CustomIngredients.Select(i => new BasketDetailsIngredientDto(i.Id.Value, i.OriginalId.Value, i.Quantity, i.Name, i.Description, i.Price)));
}
