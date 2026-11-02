using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Features.Menu.Features.Offers.Common;

public interface IRestaurantProductOfferQuery
{
    Task<RestaurantProductOffer?> ById(RestaurantProductOfferId offerId, CancellationToken cancellationToken);

    Task<RestaurantProductOffer?> ByProductId(ProductId productId, CancellationToken cancellationToken);
}
