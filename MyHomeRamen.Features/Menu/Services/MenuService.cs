using MyHomeRamen.Common.Contracts.Menu;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Services;

public class MenuService(IMenuDbContext dbContext) : IMenuService
{
    public async Task<bool> ValidateProductConfigurationAsync(
        Guid productId,
        List<Guid> selectedBaseIngredientIds,
        List<Guid> selectedCustomIngredientIds,
        CancellationToken cancellationToken)
    {
        bool exists = await dbContext.Product.Exists(p => p.Id == new ProductId(productId), cancellationToken);
        if (!exists)
        {
            return false;
        }

        Product? product = await dbContext.Product.Specification().ById(new ProductId(productId), cancellationToken);

        if (product is null)
        {
            return false;
        }

        bool baseValid = selectedBaseIngredientIds.All(id => product.BaseIngredients.Any(i => i.Id == new IngredientId(id)));
        bool customValid = selectedCustomIngredientIds.All(id => product.CustomIngredients.Any(i => i.Id == new IngredientId(id)));

        return baseValid && customValid;
    }

    public async Task<MenuProductResult?> GetProductWithSelectedIngredientsAsync(
        Guid productId,
        List<Guid> selectedBaseIngredientIds,
        List<Guid> selectedCustomIngredientIds,
        CancellationToken cancellationToken)
    {
        Product? product = await dbContext.Product.Specification().ById(new ProductId(productId), cancellationToken);

        if (product is null)
        {
            return null;
        }

        IReadOnlyList<MenuIngredientResult> baseIngredients = product.BaseIngredients
            .Where(i => selectedBaseIngredientIds.Contains(i.Id))
            .Select(i => new MenuIngredientResult(i.Id, i.Name, i.Description, i.Price))
            .ToList();

        IReadOnlyList<MenuIngredientResult> customIngredients = product.CustomIngredients
            .Where(i => selectedCustomIngredientIds.Contains(i.Id))
            .Select(i => new MenuIngredientResult(i.Id, i.Name, i.Description, i.Price))
            .ToList();

        return new MenuProductResult(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.ImageUrl,
            baseIngredients,
            customIngredients);
    }
}
