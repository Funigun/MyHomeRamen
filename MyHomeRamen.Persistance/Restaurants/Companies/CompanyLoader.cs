using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Restaurants.Features.Companies.Common;

namespace MyHomeRamen.Persistance.Restaurants;

public partial class CompanyRepository : ICompanyLoader
{
    async Task<CompanyDetails> ICompanyLoader.ById(CompanyDetailsId companyDetailsId, CancellationToken cancellationToken)
        => await First(c => c.Id == companyDetailsId, cancellationToken);

    async Task<IEnumerable<CompanyDetails>> ICompanyLoader.ByIds(IEnumerable<CompanyDetailsId> companyDetailsIds, CancellationToken cancellationToken)
        => await List(new DbQueryOptions<CompanyDetails>() { Filter = c => companyDetailsIds.Contains(c.Id) }, cancellationToken);
}
