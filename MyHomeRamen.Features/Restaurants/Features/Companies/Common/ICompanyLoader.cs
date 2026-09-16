using MyHomeRamen.Domain.Restaurants.Companies;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.Common;

public interface ICompanyLoader
{
    Task<Company> ById(CompanyId companyDetailsId, CancellationToken cancellationToken);

    Task<IEnumerable<Company>> ByIds(IEnumerable<CompanyId> companyDetailsIds, CancellationToken cancellationToken);
}
