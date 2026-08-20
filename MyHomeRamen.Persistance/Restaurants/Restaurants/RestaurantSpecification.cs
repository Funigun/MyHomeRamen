using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

namespace MyHomeRamen.Persistance.Restaurants;

public partial class RestaurantRepository : IRestaurantSpecification
{
    async Task<Restaurant> IRestaurantSpecification.ById(RestaurantId restaurantId, CancellationToken cancellationToken)
        => await First(r => r.Id == restaurantId, cancellationToken);

    async Task<IEnumerable<Restaurant>> IRestaurantSpecification.ByIds(IEnumerable<RestaurantId> restaurantIds, CancellationToken cancellationToken)
        => await List(new DbQueryOptions<Restaurant>() { Filter = r => restaurantIds.Contains(r.Id) }, cancellationToken);
}
