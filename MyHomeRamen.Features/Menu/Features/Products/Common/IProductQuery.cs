using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;
using MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;

namespace MyHomeRamen.Features.Menu.Features.Products.Common;

public interface IProductQuery
{
    Task<ProductByIdForManageDto?> GetByIdForManage(GetProductByIdForManageQueryOptions options, CancellationToken cancellationToken);

    Task<PagedResult<ProductForManageDto>> ForManage(GetProductsForManageQueryOptions options, CancellationToken cancellationToken);

    Task<bool> IsProductNameUnique(string name, CancellationToken cancellationToken);

    Task<bool> IsProductNameUniqueExcluding(string name, ProductId excludeId, CancellationToken cancellationToken);

    Task<bool> IsIngredientUsedAsBaseByProduct(IngredientId ingredientId, CancellationToken cancellationToken);

    Task<bool> IsIngredientUsedAsCustomByProduct(IngredientId ingredientId, CancellationToken cancellationToken);
}
