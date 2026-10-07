using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Restaurants.Features.Companies.Common;
using MyHomeRamen.Features.Restaurants.Features.Companies.GetDetails;

namespace MyHomeRamen.Persistance.Restaurants;

public sealed partial class CompanyRepository : ICompanyQuery
{
    public async Task<bool> HasMedia(SocialMediaId mediaId, CancellationToken cancellationToken)
        => await Exists(c => c.Media.Any(media => media.Id == mediaId), cancellationToken);

    public async Task<bool> IsNameUnique(string normalizedName, CancellationToken cancellationToken)
        => !await Exists(c => c.NormalizedName == normalizedName, cancellationToken);

    public async Task<CompanyDetailsDto?> CurrentDetails(CancellationToken cancellationToken)
    {
        return await QueryFirstOrDefault(
            restaurantsDbContext.Companies,
            new DbQueryOptions<Company, CompanyDetailsDto>
            {
                Selector = company => new CompanyDetailsDto(
                    company.Id.Value,
                    company.Name,
                    company.Description,
                    company.LogoUrl,
                    company.BusinessDetails.LegalName,
                    company.BusinessDetails.TaxId)
            },
            cancellationToken);
    }
}
