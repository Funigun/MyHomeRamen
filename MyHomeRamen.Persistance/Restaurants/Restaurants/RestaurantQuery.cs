using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

namespace MyHomeRamen.Persistance.Restaurants;

public partial class RestaurantRepository : IRestaurantQuery
{
    public async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken)
        => !await Exists(r => r.Name == name, cancellationToken);
}
