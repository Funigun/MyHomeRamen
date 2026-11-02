using MyHomeRamen.Domain.Menu.Offers;

namespace MyHomeRamen.Features.Menu.Features.Offers.Common;

public interface IRestaurantProductOfferLoader
{
    Task<RestaurantProductOffer?> ByIdForUpdate(
        RestaurantProductOfferId offerId,
        CancellationToken cancellationToken);
}
