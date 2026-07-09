using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Common.Contracts.Menu.Products.DTOs;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IProductQuery
{
    private IQueryable<Product> ProductsQuery 
        => Products.AsNoTracking();

    public async Task<List<Product>> GetByCategory(CategoryId categoryId, CancellationToken cancellationToken)
        => await ProductsQuery.Include(p => p.BaseIngredients)
                              .Where(p => p.Categories.Any(c => c.Id == categoryId))
                              .ToListAsync(cancellationToken);

    public async Task<List<Product>> GetWithAllIngredients(CancellationToken cancellationToken)
        => await ProductsQuery.Include(p => p.BaseIngredients)
                              .Include(p => p.CustomIngredients)
                              .ToListAsync(cancellationToken);

    public async Task<PagedResult<ProductForManageDto>> ForManage(
        ProductForManageFilter filter,
        PageParameters pageParameters,
        OrderParameters orderParameters,
        Expression<Func<Product, ProductForManageDto>> projection,
        CancellationToken cancellationToken)
    {
        IQueryable<Product> query = ProductsQuery;

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(p => p.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        if (filter.CategoryIds is not null && filter.CategoryIds.Any())
        {
            List<CategoryId> ids = filter.CategoryIds.Select(id => (CategoryId)id).ToList();
            query = query.Where(p => p.Categories.Any(c => ids.Contains(c.Id)));
        }

        if (filter.IngredientIds is not null && filter.IngredientIds.Any())
        {
            List<IngredientId> ids = filter.IngredientIds.Select(id => (IngredientId)id).ToList();
            query = query.Where(p =>
                p.BaseIngredients.Any(i => ids.Contains(i.Id)) ||
                p.CustomIngredients.Any(i => ids.Contains(i.Id)));
        }

        if (filter.PriceFrom.HasValue)
        {
            query = query.Where(p => p.Price >= filter.PriceFrom.Value);
        }

        if (filter.PriceTo.HasValue)
        {
            query = query.Where(p => p.Price <= filter.PriceTo.Value);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        Expression<Func<Product, object>> keySelector = orderParameters.SortOrder switch
        {
            "Price" => c => c.Price,
            "Name" => c => c.Name,
            _ => pageParameters => pageParameters.Name
        };

        query = query.Paged(pageParameters.PageNumber, pageParameters.PageSize)
                     .OrderedBy(keySelector, orderParameters.SortOrder);

        List<ProductForManageDto> products = await query.Select(projection).ToListAsync(cancellationToken);

        return new(totalCount, products);
    }

    public async Task<bool> IsProductNameUnique(string name, CancellationToken cancellationToken)
        => await ProductsQuery.AnyAsync(p => p.Name.ToLower() != name.ToLower(), cancellationToken);

    public async Task<bool> IsProductNameUniqueExcluding(string name, ProductId excludeId, CancellationToken cancellationToken)
        => !await ProductsQuery.AnyAsync(p => p.Id != excludeId && p.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<bool> IsCategoryUsedByProduct(CategoryId categoryId, CancellationToken cancellationToken)
        => await ProductsQuery.AnyAsync(p => p.Categories.Any(c => c.Id == categoryId), cancellationToken);

    public async Task<bool> IsIngredientUsedAsBaseByProduct(IngredientId ingredientId, CancellationToken cancellationToken)
        => await ProductsQuery.AnyAsync(p => p.BaseIngredients.Any(i => i.Id == ingredientId), cancellationToken);

    public async Task<bool> IsIngredientUsedAsCustomByProduct(IngredientId ingredientId, CancellationToken cancellationToken)
        => await ProductsQuery.AnyAsync(p => p.CustomIngredients.Any(i => i.Id == ingredientId), cancellationToken);

    async Task<Product> IProductQuery.ById(ProductId productId, CancellationToken cancellationToken)
        => await ProductsQuery.Include(p => p.BaseIngredients)
                              .Include(p => p.CustomIngredients)
                              .Include(p => p.Categories)
                              .FirstAsync(p => p.Id == productId, cancellationToken);

}
