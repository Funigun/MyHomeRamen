using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

public interface IRestaurantRepository : IRepository<Restaurant, RestaurantId>
{
    IRestaurantQuery Query();

    IRestaurantSpecification Specification();
}
