using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Menu.Features.Offers.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public sealed partial class RestaurantProductOfferRepository(MenuDbContext menuDbContext, ICacheService cacheService) : BaseRepository<RestaurantProductOffer, RestaurantProductOfferId>(menuDbContext, cacheService), IRestaurantProductOfferRepository
{
    public IRestaurantProductOfferQuery Query() => this;

    public IRestaurantProductOfferLoader Load() => this;
}
