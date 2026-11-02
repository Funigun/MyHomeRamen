using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetRestaurantOptions;

public sealed record GetRestaurantOptionsQuery : IQuery<GetRestaurantOptionsResponse>;

public sealed record RestaurantOptionDto(
    Guid Id,
    string Name,
    string Street,
    string City,
    string ZipCode,
    double Latitude,
    double Longitude);

public sealed class GetRestaurantOptionsHandler(IRestaurantDbContext dbContext)
    : IRequestHandler<GetRestaurantOptionsQuery, GetRestaurantOptionsResponse>
{
    public async Task<GetRestaurantOptionsResponse> Handle(
        GetRestaurantOptionsQuery query,
        CancellationToken cancellationToken)
    {
        IEnumerable<RestaurantOptionDto> restaurants = await dbContext.Restaurant.Query().ActiveOptions(cancellationToken);
        return new GetRestaurantOptionsResponse(restaurants);
    }
}
