using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardForManage;
using MyHomeRamen.Features.Menu.Features.Offers.Common;
using MyHomeRamen.Features.Menu.Features.Offers.GetRestaurantOffersForManage;

namespace MyHomeRamen.Persistance.Menu;

public partial class RestaurantProductOfferRepository : IRestaurantProductOfferQuery
{
    public async Task<PagedResult<RestaurantProductOfferForManageDto>> ForManage(
        GetRestaurantOffersForManageQueryOptions options,
        CancellationToken cancellationToken)
        => await QueryPaged(
            menuDbContext.RestaurantProductOffers
                .AsNoTracking()
                .Where(offer => offer.RestaurantId == options.RestaurantId),
            options,
            cancellationToken);

    public async Task<RestaurantProductOffer?> ById(RestaurantProductOfferId offerId, CancellationToken cancellationToken)
        => await menuDbContext.RestaurantProductOffers.AsNoTracking()
            .Where(offer => offer.Id == offerId
                && offer.Status == RestaurantProductOfferStatus.Active
                && offer.Product.Status == ProductStatus.Active)
            .Include(offer => offer.Product)
                .ThenInclude(product => product.BaseIngredients)
            .Include(offer => offer.Product)
                .ThenInclude(product => product.CustomIngredients)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<RestaurantProductOffer?> ByProductId(ProductId productId, CancellationToken cancellationToken)
        => await menuDbContext.RestaurantProductOffers.AsNoTracking()
            .Where(offer => offer.ProductId == productId
                && offer.Status == RestaurantProductOfferStatus.Active
                && offer.Product.Status == ProductStatus.Active)
            .Include(offer => offer.Product)
                .ThenInclude(product => product.BaseIngredients)
            .Include(offer => offer.Product)
                .ThenInclude(product => product.CustomIngredients)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<MenuCardOfferChoiceDto>> EligibleForMenuCard(
        Guid restaurantId,
        CancellationToken cancellationToken)
        => await menuDbContext.RestaurantProductOffers
            .AsNoTracking()
            .Where(offer => offer.RestaurantId == restaurantId
                && offer.Status == RestaurantProductOfferStatus.Active
                && offer.Product.Status == ProductStatus.Active)
            .OrderBy(offer => offer.Product.Name)
            .Select(offer => new MenuCardOfferChoiceDto(
                offer.Id.Value,
                offer.Product.Name,
                offer.Product.Description,
                offer.Product.ImageUrl,
                offer.Product.Categories
                    .OrderBy(category => category.Name)
                    .Select(category => category.Name)
                    .ToArray(),
                offer.Price,
                offer.Status == RestaurantProductOfferStatus.Active
                    && offer.Product.Status == ProductStatus.Active,
                offer.Status.ToString(),
                offer.Product.Status.ToString()))
            .ToListAsync(cancellationToken);
}
