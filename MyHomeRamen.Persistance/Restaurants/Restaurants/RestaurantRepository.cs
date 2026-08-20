using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Restaurants;

public sealed partial class RestaurantRepository(RestaurantsDbContext restaurantsDbContext, ICacheService cacheService) : BaseRepository<Restaurant, RestaurantId>(restaurantsDbContext, cacheService), IRestaurantRepository
{
    public IRestaurantQuery Query() => this;

    public IRestaurantLoader Load() => this;
}
