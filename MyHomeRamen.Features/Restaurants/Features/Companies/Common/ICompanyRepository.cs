using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.Common;

public interface ICompanyRepository : IRepository<CompanyDetails, CompanyDetailsId>
{
    ICompanyQuery Query();

    ICompanyLoader Load();
}
