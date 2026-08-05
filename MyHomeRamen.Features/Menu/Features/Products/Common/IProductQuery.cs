using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.Products.GetProductById;
using MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;
using MyHomeRamen.Features.Menu.Features.Products.GetProductsByCategory;
using MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;

namespace MyHomeRamen.Features.Menu.Features.Products.Common;

public interface IProductQuery
{
    Task<Product> ById(ProductId productId, CancellationToken cancellationToken);

    Task<ProductByIdDto?> GetById(GetProductByIdQueryOptions options, CancellationToken cancellationToken);

    Task<ProductByIdForManageDto?> GetByIdForManage(GetProductByIdForManageQueryOptions options, CancellationToken cancellationToken);

    Task<IEnumerable<ProductByCategoryDto>> GetByCategory(GetProductsByCategoryQueryOptions options, CancellationToken cancellationToken);

    Task<PagedResult<ProductForManageDto>> ForManage(GetProductsForManageQueryOptions options, CancellationToken cancellationToken);

    Task<bool> IsProductNameUnique(string name, CancellationToken cancellationToken);

    Task<bool> IsProductNameUniqueExcluding(string name, ProductId excludeId, CancellationToken cancellationToken);

    Task<bool> IsIngredientUsedAsBaseByProduct(IngredientId ingredientId, CancellationToken cancellationToken);

    Task<bool> IsIngredientUsedAsCustomByProduct(IngredientId ingredientId, CancellationToken cancellationToken);
}
