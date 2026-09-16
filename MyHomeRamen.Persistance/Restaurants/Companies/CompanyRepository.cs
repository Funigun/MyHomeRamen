using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Restaurants.Features.Companies.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Restaurants;

public sealed partial class CompanyRepository(RestaurantsDbContext restaurantsDbContext, ICacheService cacheService) : BaseRepository<Company, CompanyId>(restaurantsDbContext, cacheService), ICompanyRepository
{
    public ICompanyQuery Query() => this;

    public ICompanyLoader Load() => this;
}
