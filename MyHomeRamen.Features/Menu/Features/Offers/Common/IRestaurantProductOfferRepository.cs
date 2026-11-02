using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Offers.Common;

public interface IRestaurantProductOfferRepository : IRepository<RestaurantProductOffer, RestaurantProductOfferId>
{
    IRestaurantProductOfferQuery Query();

    IRestaurantProductOfferLoader Load();
}
