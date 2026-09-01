using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Restaurants.Features.Companies.Common;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

namespace MyHomeRamen.Features.Restaurants.Features.Abstractions;

public interface IRestaurantDbContext : IUnitOfWork
{
    ICompanyRepository Company { get; }

    IRestaurantRepository Restaurant { get; }
}
