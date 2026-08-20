using MyHomeRamen.Domain.Restaurants.Companies;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.Common;

public interface ICompanyLoader
{
    Task<CompanyDetails> ById(CompanyDetailsId companyDetailsId, CancellationToken cancellationToken);

    Task<IEnumerable<CompanyDetails>> ByIds(IEnumerable<CompanyDetailsId> companyDetailsIds, CancellationToken cancellationToken);
}
