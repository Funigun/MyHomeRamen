using FluentValidation;
using FluentValidation.Results;
using MyHomeRamen.Domain.Identity.CompanyMemberships;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Features.Identity.Services;

public sealed class IdentityService(IKeycloakAdminService keycloakAdminService, IIdentityDbContext identityDbContext, IValidator<CompanyOwnerDto> validator) : IIdentityService
{
    public async Task<CompanyOwnerRegistrationResult> RegisterCompanyOwnerAsync(CompanyOwnerDto companyOwner, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(companyOwner, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            string error = string.Join(" ", validationResult.Errors.Select(validationError => validationError.ErrorMessage));
            return new CompanyOwnerRegistrationResult(null, null, error);
        }

        CompanyMembership? existingMembership = await identityDbContext.CompanyMembership.ByIdempotencyKey(companyOwner.IdempotencyKey, cancellationToken);
        
        if (existingMembership is not null)
        {
            return new CompanyOwnerRegistrationResult(existingMembership.UserId.Value, existingMembership.CompanyId, null);
        }

        Role customerRole = await identityDbContext.Role.Load().ByName(RoleConstants.Customer, cancellationToken)
                        ?? throw new InvalidOperationException("Customer role was not found.");

        Role ownerRole = await identityDbContext.Role.Load().ByName(RoleConstants.CompanyOwner, cancellationToken)
                     ?? throw new InvalidOperationException("Company owner role was not found.");

        KeycloakUserDto keycloakUser = new()
        {
            Username = companyOwner.UserName,
            Email = companyOwner.Email,
            FirstName = companyOwner.FirstName,
            LastName = companyOwner.LastName,
            Enabled = true,
            Credentials = [new KeycloakCredentialDto { Type = "password", Value = companyOwner.Password, Temporary = false }]
        };

        string keycloakUserId = await keycloakAdminService.CreateUserAsync(keycloakUser, cancellationToken);
        
        User user = User.Create(keycloakUserId, companyOwner.UserName, companyOwner.FirstName, companyOwner.LastName, companyOwner.Email, companyOwner.PhoneNumber, customerRole);

        user.AddRole(ownerRole);

        CompanyMembership membership = CompanyMembership.CreateOwner(user.Id, companyOwner.CompanyId, ownerRole.Id, companyOwner.IdempotencyKey);

        identityDbContext.User.Add(user);
        identityDbContext.CompanyMembership.Add(membership);
        await identityDbContext.SaveChangesAsync(cancellationToken);

        return new CompanyOwnerRegistrationResult(user.Id.Value, companyOwner.CompanyId, null);
    }
}
