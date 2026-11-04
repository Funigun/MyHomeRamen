namespace MyHomeRamen.Features.Identity.ExternalApi;

public interface IIdentityService
{
    Task<CompanyOwnerRegistrationResult> RegisterCompanyOwnerAsync(CompanyOwnerDto companyOwner, CancellationToken cancellationToken);

    Task<Guid> CreateRestaurantAdminRoleAsync(Guid restaurantId, CancellationToken cancellationToken);

    Task<bool> IsCompanyOwnerConfiguredAsync(CancellationToken cancellationToken);
}
