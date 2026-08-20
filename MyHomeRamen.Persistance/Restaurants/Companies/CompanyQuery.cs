using MyHomeRamen.Features.Restaurants.Features.Companies.Common;

namespace MyHomeRamen.Persistance.Restaurants;

public partial class CompanyRepository : ICompanyQuery
{
    public async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken)
        => !await Exists(c => c.Name == name, cancellationToken);
}
