namespace MyHomeRamen.Common.Contracts.Menu;

public interface IMenuService
{
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
