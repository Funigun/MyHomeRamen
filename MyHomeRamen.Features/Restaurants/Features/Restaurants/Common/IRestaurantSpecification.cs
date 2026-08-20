using MyHomeRamen.Domain.Restaurants.Restaurants;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

public interface IRestaurantSpecification
{
    Task<Restaurant> ById(RestaurantId restaurantId, CancellationToken cancellationToken);

    Task<IEnumerable<Restaurant>> ByIds(IEnumerable<RestaurantId> restaurantIds, CancellationToken cancellationToken);
}
