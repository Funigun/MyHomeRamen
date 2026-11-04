using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.Common;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.GetRestaurantIngredientPricesForManage;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public sealed partial class RestaurantIngredientPriceRepository(MenuDbContext menuDbContext,ICacheService cacheService)
                          : BaseRepository<RestaurantIngredientPrice, RestaurantIngredientPriceId>(menuDbContext, cacheService), IRestaurantIngredientPriceRepository, IRestaurantIngredientPriceQuery
{
    public IRestaurantIngredientPriceQuery Query() => this;

    public IRestaurantIngredientPriceLoader Load() => this;

    public async Task<IReadOnlyCollection<RestaurantIngredientPriceDto>> ByIngredientIds(IEnumerable<IngredientId> ingredientIds, CancellationToken cancellationToken)
    {
        HashSet<IngredientId> requestedIds = ingredientIds.ToHashSet();

        return await menuDbContext.RestaurantIngredientPrices.AsNoTracking()
                                                             .Where(price => requestedIds.Contains(price.IngredientId))
                                                             .Select(price => new RestaurantIngredientPriceDto(price.IngredientId.Value, price.Price))
                                                             .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<RestaurantIngredientPriceForManageDto>> ForManage(
        GetRestaurantIngredientPricesForManageQueryOptions options,
        CancellationToken cancellationToken)
        => await QueryPaged(
            menuDbContext.RestaurantIngredientPrices
                .AsNoTracking()
                .Where(price => price.RestaurantId == options.RestaurantId),
            options,
            cancellationToken);
}
