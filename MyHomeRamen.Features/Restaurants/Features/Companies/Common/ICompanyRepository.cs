using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.Common;

public interface ICompanyRepository : IRepository<Company, CompanyId>
{
    ICompanyQuery Query();

    ICompanyLoader Load();
}
