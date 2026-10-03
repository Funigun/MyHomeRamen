using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Restaurants;

public sealed partial class RestaurantRepository(RestaurantsDbContext restaurantsDbContext, ICacheService cacheService) : BaseRepository<Restaurant, RestaurantId>(restaurantsDbContext, cacheService), IRestaurantRepository
{
    public IRestaurantQuery Query() => this;

    public IRestaurantLoader Load() => this;

    public async Task<RestaurantDetailsDto?> Details(RestaurantId restaurantId, CancellationToken cancellationToken)
        => await restaurantsDbContext.Restaurants.AsNoTracking().Where(r => r.Id == restaurantId).Select(r => new RestaurantDetailsDto(r.Id.Value, r.Name, r.IsActive, r.Address.Street, r.Address.City, r.Address.ZipCode, r.Address.Location.Latitude, r.Address.Location.Longitude, r.ContactDetails == null ? null : r.ContactDetails.Phone, r.ContactDetails == null ? null : r.ContactDetails.Email, r.BankAccount == null ? null : r.BankAccount.AccountNumber, r.BankAccount == null ? null : r.BankAccount.BankName, r.BankAccount == null ? null : r.BankAccount.RoutingNumber)).FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<RestaurantListItemDto>> Available(CancellationToken cancellationToken)
        => await restaurantsDbContext.Restaurants.AsNoTracking().OrderBy(r => r.CreatedOn).Select(r => new RestaurantListItemDto(r.Id.Value, r.Name, r.IsActive, r.Address.Street, r.Address.City, r.Address.ZipCode, r.Address.Location.Latitude, r.Address.Location.Longitude, r.ContactDetails == null ? null : r.ContactDetails.Phone, r.ContactDetails == null ? null : r.ContactDetails.Email)).ToListAsync(cancellationToken);
}



