using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Restaurants.Features.Companies.Common;

namespace MyHomeRamen.Persistance.Restaurants;

public partial class CompanyRepository : ICompanyLoader
{
    async Task<Company> ICompanyLoader.ById(CompanyId companyDetailsId, CancellationToken cancellationToken)
        => await First(c => c.Id == companyDetailsId, cancellationToken);

    async Task<IEnumerable<Company>> ICompanyLoader.ByIds(IEnumerable<CompanyId> companyDetailsIds, CancellationToken cancellationToken)
        => await List(new DbQueryOptions<Company>() { Filter = c => companyDetailsIds.Contains(c.Id) }, cancellationToken);
}
