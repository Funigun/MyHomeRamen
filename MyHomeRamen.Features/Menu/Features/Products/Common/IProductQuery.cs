using System.Linq.Expressions;
using MyHomeRamen.Common.Contracts.Menu.Products.DTOs;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;

namespace MyHomeRamen.Features.Menu.Features.Products.Common;

public interface IProductQuery
{
    Task<Product> ById(ProductId productId, CancellationToken cancellationToken);

    Task<List<Product>> GetByCategory(CategoryId categoryId, CancellationToken cancellationToken);

    Task<List<Product>> GetWithAllIngredients(CancellationToken cancellationToken);

    Task<PagedResult<ProductForManageDto>> ForManage(
        ProductForManageFilter filter,
        PageParameters pageParameters,
        OrderParameters orderParameters,
        Expression<Func<Product, ProductForManageDto>> projection,
        CancellationToken cancellationToken);

    Task<bool> IsProductNameUnique(string name, CancellationToken cancellationToken);

    Task<bool> IsProductNameUniqueExcluding(string name, ProductId excludeId, CancellationToken cancellationToken);

    Task<bool> IsCategoryUsedByProduct(CategoryId categoryId, CancellationToken cancellationToken);

    Task<bool> IsIngredientUsedAsBaseByProduct(IngredientId ingredientId, CancellationToken cancellationToken);

    Task<bool> IsIngredientUsedAsCustomByProduct(IngredientId ingredientId, CancellationToken cancellationToken);
}
