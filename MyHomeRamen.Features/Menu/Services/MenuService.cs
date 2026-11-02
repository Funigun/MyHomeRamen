using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;
using MyHomeRamen.Features.Restaurants.ExternalApi;

namespace MyHomeRamen.Features.Menu.Services;

public sealed class MenuService(
    IMenuDbContext dbContext,
    ICompanyContext companyContext,
    IRestaurantService restaurantService) : IMenuService
{
    public async Task<MenuOfferResult?> GetOfferAsync(
        Guid restaurantId,
        Guid offerId,
        CancellationToken cancellationToken)
    {
        if (!await IsValidRestaurantContext(restaurantId, cancellationToken))
        {
            return null;
        }

        RestaurantProductOffer? offer = await dbContext.RestaurantProductOffer.Query()
            .ById(new RestaurantProductOfferId(offerId), cancellationToken);

        if (offer is null)
        {
            return null;
        }

        IReadOnlyCollection<RestaurantIngredientPriceDto> prices = await dbContext.RestaurantIngredientPrice.Query()
            .ByIngredientIds(
                offer.Product.CustomIngredients.Select(ingredient => ingredient.Id),
                cancellationToken);
        Dictionary<Guid, decimal> pricesByIngredientId = prices.ToDictionary(price => price.IngredientId, price => price.Price);

        return new MenuOfferResult(
            offer.RestaurantId,
            offer.Id.Value,
            offer.ProductId.Value,
            offer.Product.Name,
            offer.Product.Description,
            offer.Price,
            offer.Product.ImageUrl,
            offer.Product.BaseIngredients
                .Select(ingredient => new MenuOfferIngredientResult(ingredient.Id.Value, ingredient.Name, 0))
                .ToArray(),
            offer.Product.CustomIngredients
                .Where(ingredient => pricesByIngredientId.ContainsKey(ingredient.Id.Value))
                .Select(ingredient => new MenuOfferIngredientResult(
                    ingredient.Id.Value,
                    ingredient.Name,
                    pricesByIngredientId[ingredient.Id.Value]))
                .ToArray());
    }

    public async Task<bool> ValidateProductConfigurationAsync(
        Guid productId,
        List<Guid> selectedBaseIngredientIds,
        List<Guid> selectedCustomIngredientIds,
        CancellationToken cancellationToken)
    {
        Guid? restaurantId = companyContext.RestaurantId;
        if (!restaurantId.HasValue || !await IsValidRestaurantContext(restaurantId.Value, cancellationToken))
        {
            return false;
        }

        RestaurantProductOffer? offer = await dbContext.RestaurantProductOffer.Query()
            .ByProductId(new ProductId(productId), cancellationToken);
        if (offer is null)
        {
            return false;
        }

        Product product = offer.Product;
        bool baseValid = selectedBaseIngredientIds.All(id => product.BaseIngredients.Any(i => i.Id == new IngredientId(id)));
        IReadOnlyCollection<RestaurantIngredientPriceDto> prices = await dbContext.RestaurantIngredientPrice.Query()
            .ByIngredientIds(
                selectedCustomIngredientIds.Select(id => new IngredientId(id)),
                cancellationToken);
        HashSet<Guid> pricedIngredientIds = prices.Select(price => price.IngredientId).ToHashSet();
        bool customValid = selectedCustomIngredientIds.All(id =>
            product.CustomIngredients.Any(ingredient => ingredient.Id == new IngredientId(id))
            && pricedIngredientIds.Contains(id));

        return baseValid && customValid;
    }

    public async Task<MenuProductResult?> GetProductWithSelectedIngredientsAsync(
        Guid productId,
        List<Guid> selectedBaseIngredientIds,
        List<Guid> selectedCustomIngredientIds,
        CancellationToken cancellationToken)
    {
        Guid? restaurantId = companyContext.RestaurantId;
        if (!restaurantId.HasValue || !await IsValidRestaurantContext(restaurantId.Value, cancellationToken))
        {
            return null;
        }

        RestaurantProductOffer? offer = await dbContext.RestaurantProductOffer.Query()
            .ByProductId(new ProductId(productId), cancellationToken);
        if (offer is null)
        {
            return null;
        }

        Product product = offer.Product;
        IReadOnlyCollection<RestaurantIngredientPriceDto> prices = await dbContext.RestaurantIngredientPrice.Query()
            .ByIngredientIds(
                selectedCustomIngredientIds.Select(id => new IngredientId(id)),
                cancellationToken);
        Dictionary<Guid, decimal> pricesByIngredientId = prices.ToDictionary(price => price.IngredientId, price => price.Price);
        bool baseIngredientsAvailable = selectedBaseIngredientIds.All(id =>
            product.BaseIngredients.Any(ingredient => ingredient.Id == new IngredientId(id)));
        bool customIngredientsAvailable = selectedCustomIngredientIds.All(id =>
            product.CustomIngredients.Any(ingredient => ingredient.Id == new IngredientId(id))
            && pricesByIngredientId.ContainsKey(id));

        if (!baseIngredientsAvailable || !customIngredientsAvailable)
        {
            return null;
        }

        IReadOnlyList<MenuIngredientResult> baseIngredients = product.BaseIngredients
            .Where(i => selectedBaseIngredientIds.Contains(i.Id))
            .Select(i => new MenuIngredientResult(i.Id, i.Name, i.Description, 0))
            .ToList();

        IReadOnlyList<MenuIngredientResult> customIngredients = product.CustomIngredients
            .Where(i => selectedCustomIngredientIds.Contains(i.Id) && pricesByIngredientId.ContainsKey(i.Id.Value))
            .Select(i => new MenuIngredientResult(i.Id, i.Name, i.Description, pricesByIngredientId[i.Id.Value]))
            .ToList();

        return new MenuProductResult(
            product.Id,
            product.Name,
            product.Description,
            offer.Price,
            product.ImageUrl,
            baseIngredients,
            customIngredients);
    }

    private async Task<bool> IsValidRestaurantContext(Guid restaurantId, CancellationToken cancellationToken)
    {
        if (companyContext.RestaurantId != restaurantId)
        {
            return false;
        }

        RestaurantInfo? restaurant = await restaurantService.GetRestaurantAsync(restaurantId, cancellationToken);
        return restaurant is { IsActive: true } && restaurant.CompanyId == companyContext.CompanyId;
    }
}
