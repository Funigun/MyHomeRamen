using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Common.Contracts.Menu.Products.DTOs;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;

namespace MyHomeRamen.Persistance.Menu;

public partial class ProductRepository : IProductQuery
{
    public async Task<List<Product>> GetByCategory(CategoryId categoryId, CancellationToken cancellationToken)
        => await menuDbContext.Products.AsNoTracking()
                              .Include(p => p.BaseIngredients)
                              .Where(p => p.Categories.Any(c => c.Id == categoryId))
                              .ToListAsync(cancellationToken);

    public async Task<List<Product>> GetWithAllIngredients(CancellationToken cancellationToken)
        => await menuDbContext.Products.AsNoTracking()
                              .Include(p => p.BaseIngredients)
                              .Include(p => p.CustomIngredients)
                              .ToListAsync(cancellationToken);

    public async Task<PagedResult<ProductForManageDto>> ForManage(
        ProductForManageFilter filter,
        PageParameters pageParameters,
        OrderParameters orderParameters,
        Expression<Func<Product, ProductForManageDto>> projection,
        CancellationToken cancellationToken)
    {
        string? nameFilter = string.IsNullOrWhiteSpace(filter.Name) ? null : filter.Name.ToLower();

        List<CategoryId>? categoryIds = filter.CategoryIds is not null && filter.CategoryIds.Any()
            ? filter.CategoryIds.Select(id => (CategoryId)id).ToList()
            : null;

        List<IngredientId>? ingredientIds = filter.IngredientIds is not null && filter.IngredientIds.Any()
            ? filter.IngredientIds.Select(id => (IngredientId)id).ToList()
            : null;

        decimal? priceFrom = filter.PriceFrom;
        decimal? priceTo = filter.PriceTo;

        Expression<Func<Product, bool>> predicate = p =>
            (nameFilter == null || p.Name.ToLower().Contains(nameFilter)) &&
            (categoryIds == null || p.Categories.Any(c => categoryIds.Contains(c.Id))) &&
            (ingredientIds == null || p.BaseIngredients.Any(i => ingredientIds.Contains(i.Id)) || p.CustomIngredients.Any(i => ingredientIds.Contains(i.Id))) &&
            (priceFrom == null || p.Price >= priceFrom.Value) &&
            (priceTo == null || p.Price <= priceTo.Value);

        Expression<Func<Product, object>> orderBy = orderParameters.SortBy switch
        {
            "Price" => p => p.Price,
            "Name" => p => p.Name,
            _ => p => p.Name
        };

        DbQueryOptions<Product> query = new() {Filter = predicate, OrderBy = orderBy, OrderDirection = orderParameters.SortOrder};
        PagedDbQueryOptions<Product, ProductForManageDto> paged = new PagedDbQueryOptions<Product, ProductForManageDto>()
        {
            Filter = query.Filter,
            OrderBy = orderBy,
            OrderDirection = query.OrderDirection,
            PageNumber = pageParameters.PageNumber,
            PageSize = pageParameters.PageSize,
            Selector = projection
        };

        return await QueryPaged(menuDbContext.Products, paged, cancellationToken);
    }

    public async Task<bool> IsProductNameUnique(string name, CancellationToken cancellationToken)
        => !await Exists(p => p.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<bool> IsProductNameUniqueExcluding(string name, ProductId excludeId, CancellationToken cancellationToken)
        => !await Exists(p => p.Id != excludeId && p.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<bool> IsCategoryUsedByProduct(CategoryId categoryId, CancellationToken cancellationToken)
        => await Exists(p => p.Categories.Any(c => c.Id == categoryId), cancellationToken);

    public async Task<bool> IsIngredientUsedAsBaseByProduct(IngredientId ingredientId, CancellationToken cancellationToken)
        => await Exists(p => p.BaseIngredients.Any(i => i.Id == ingredientId), cancellationToken);

    public async Task<bool> IsIngredientUsedAsCustomByProduct(IngredientId ingredientId, CancellationToken cancellationToken)
        => await Exists(p => p.CustomIngredients.Any(i => i.Id == ingredientId), cancellationToken);

    async Task<Product> IProductQuery.ById(ProductId productId, CancellationToken cancellationToken)
        => await menuDbContext.Products.AsNoTracking()
                              .Include(p => p.BaseIngredients)
                              .Include(p => p.CustomIngredients)
                              .Include(p => p.Categories)
                              .FirstAsync(p => p.Id == productId, cancellationToken);
}
