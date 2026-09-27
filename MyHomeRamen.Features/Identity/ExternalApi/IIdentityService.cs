namespace MyHomeRamen.Features.Identity.ExternalApi;

public interface IIdentityService
{
    Task<CompanyOwnerRegistrationResult> RegisterCompanyOwnerAsync(CompanyOwnerDto companyOwner, CancellationToken cancellationToken);
}
