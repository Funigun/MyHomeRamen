using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;
using MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;
using MyHomeRamen.Features.Menu.Features.Offers.GetRestaurantOffersForManage;

namespace MyHomeRamen.Persistance.Menu;

public partial class ProductRepository : IProductQuery
{
    public async Task<ProductByIdForManageDto?> GetByIdForManage(GetProductByIdForManageQueryOptions options, CancellationToken cancellationToken)
        => await QueryFirstOrDefault(
            menuDbContext.Products.AsNoTracking()
                .Include(p => p.BaseIngredients)
                .Include(p => p.CustomIngredients)
                .Include(p => p.Categories)
                .AsSplitQuery(),
            options,
            cancellationToken);

    public async Task<PagedResult<ProductForManageDto>> ForManage(GetProductsForManageQueryOptions options, CancellationToken cancellationToken)
        => await QueryPaged(menuDbContext.Products.AsNoTracking()
                              .Include(p => p.BaseIngredients)
                              .Include(p => p.CustomIngredients)
                              .Include(p => p.Categories)
                              .AsSplitQuery(), options, cancellationToken);

    public async Task<IReadOnlyList<EligibleProductForOfferDto>> EligibleForOffers(
        Guid restaurantId,
        CancellationToken cancellationToken)
        => await menuDbContext.Products.AsNoTracking()
            .Where(product => product.Status == ProductStatus.Active
                && !menuDbContext.RestaurantProductOffers.Any(offer =>
                    offer.RestaurantId == restaurantId && offer.ProductId == product.Id))
            .OrderBy(product => product.Name)
            .Select(product => new EligibleProductForOfferDto(product.Id.Value, product.Name))
            .ToListAsync(cancellationToken);

    public async Task<bool> IsProductNameUnique(string name, CancellationToken cancellationToken)
        => !await Exists(p => p.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<bool> IsProductNameUniqueExcluding(string name, ProductId excludeId, CancellationToken cancellationToken)
        => !await Exists(p => p.Id != excludeId && p.Name.ToLower() == name.ToLower(), cancellationToken);

    public async Task<bool> IsIngredientUsedAsBaseByProduct(IngredientId ingredientId, CancellationToken cancellationToken)
        => await Exists(p => p.BaseIngredients.Any(i => i.Id == ingredientId), cancellationToken);

    public async Task<bool> IsIngredientUsedAsCustomByProduct(IngredientId ingredientId, CancellationToken cancellationToken)
        => await Exists(p => p.CustomIngredients.Any(i => i.Id == ingredientId), cancellationToken);

}
