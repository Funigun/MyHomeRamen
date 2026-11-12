using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardForManage;
using MyHomeRamen.Features.Menu.Features.Offers.GetRestaurantOffersForManage;
using MyHomeRamen.Features.Common.Endpoints.Models;

namespace MyHomeRamen.Features.Menu.Features.Offers.Common;

public interface IRestaurantProductOfferQuery
{
    Task<PagedResult<RestaurantProductOfferForManageDto>> ForManage(
        GetRestaurantOffersForManageQueryOptions options,
        CancellationToken cancellationToken);

    Task<RestaurantProductOffer?> ById(RestaurantProductOfferId offerId, CancellationToken cancellationToken);

    Task<RestaurantProductOffer?> ByProductId(ProductId productId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MenuCardOfferChoiceDto>> EligibleForMenuCard(
        Guid restaurantId,
        CancellationToken cancellationToken);
}
