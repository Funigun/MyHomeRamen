namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

public interface IRestaurantQuery
{
    Task<bool> IsNameUnique(string name, CancellationToken cancellationToken);
}
