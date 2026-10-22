using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.Setup;

public sealed record GetCompanySetupStatusQuery : IQuery<CompanySetupStatusResponse>;

public sealed record CompanySetupStatusResponse(bool IsOwnerConfigured, bool CompanyExists, bool RestaurantExists)
{
    public bool IsComplete => IsOwnerConfigured && CompanyExists && RestaurantExists;
}

public sealed class GetCompanySetupStatusHandler(
    IRestaurantDbContext restaurantDbContext,
    IIdentityService identityService)
    : IRequestHandler<GetCompanySetupStatusQuery, CompanySetupStatusResponse>
{
    public async Task<CompanySetupStatusResponse> Handle(GetCompanySetupStatusQuery query, CancellationToken cancellationToken)
    {
        bool isOwnerConfigured = await identityService.IsCompanyOwnerConfiguredAsync(cancellationToken);
        bool companyExists = await restaurantDbContext.Company.Exists(_ => true, cancellationToken);
        bool restaurantExists = await restaurantDbContext.Restaurant.Exists(_ => true, cancellationToken);

        return new CompanySetupStatusResponse(isOwnerConfigured, companyExists, restaurantExists);
    }
}
