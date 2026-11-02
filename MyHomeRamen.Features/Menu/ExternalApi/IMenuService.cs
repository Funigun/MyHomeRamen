namespace MyHomeRamen.Features.Menu.ExternalApi;

public interface IMenuService
{
    Task<MenuOfferResult?> GetOfferAsync(
        Guid restaurantId,
        Guid offerId,
        CancellationToken cancellationToken);

    Task<MenuOfferResult?> GetOfferForBasketAsync(
        Guid restaurantId,
        Guid offerId,
        CancellationToken cancellationToken);

    Task<MenuProductResult?> GetProductWithSelectedIngredientsAsync(
        Guid productId,
        List<Guid> selectedBaseIngredientIds,
        List<Guid> selectedCustomIngredientIds,
        CancellationToken cancellationToken);

    Task<bool> ValidateProductConfigurationAsync(
        Guid productId,
        List<Guid> selectedBaseIngredientIds,
        List<Guid> selectedCustomIngredientIds,
        CancellationToken cancellationToken);
}
