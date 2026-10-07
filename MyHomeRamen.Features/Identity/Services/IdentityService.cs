using FluentValidation;
using FluentValidation.Results;
using MyHomeRamen.Domain.Identity.CompanyMemberships;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Features.Identity.Services;

public sealed class IdentityService(
    IKeycloakAdminService keycloakAdminService,
    IIdentityDbContext identityDbContext,
    IValidator<CompanyOwnerDto> companyOwnerValidator,
    IValidator<RestaurantAdminRegistrationDto> restaurantAdminValidator) : IIdentityService
{
    public async Task<CompanyOwnerRegistrationResult> RegisterCompanyOwnerAsync(CompanyOwnerDto companyOwner, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await companyOwnerValidator.ValidateAsync(companyOwner, cancellationToken);
        
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

    public async Task<RestaurantAdminRegistrationResult> RegisterRestaurantAdminAsync(RestaurantAdminRegistrationDto restaurantAdmin, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await restaurantAdminValidator.ValidateAsync(restaurantAdmin, cancellationToken);

        if (!validationResult.IsValid)
        {
            string error = string.Join(" ", validationResult.Errors.Select(validationError => validationError.ErrorMessage));
            return new RestaurantAdminRegistrationResult(null, error);
        }

        Role customerRole = await identityDbContext.Role.Load().ByName(RoleConstants.Customer, cancellationToken)
                        ?? throw new InvalidOperationException("Customer role was not found.");

        Role restaurantAdminRole = await identityDbContext.Role.Load().ByName(RoleConstants.RestaurantAdmin, cancellationToken)
                               ?? throw new InvalidOperationException("Restaurant admin role was not found.");

        CompanyMembership? existingMembership = await identityDbContext.CompanyMembership.ByIdempotencyKey(restaurantAdmin.IdempotencyKey, cancellationToken);

        if (existingMembership is not null)
        {
            if (existingMembership.CompanyId == restaurantAdmin.CompanyId
                && existingMembership.RestaurantId == restaurantAdmin.RestaurantId
                && existingMembership.RoleId == restaurantAdminRole.Id)
            {
                return new RestaurantAdminRegistrationResult(existingMembership.Id.Value, null);
            }

            return new RestaurantAdminRegistrationResult(null, "Idempotency key has already been used for a different restaurant admin assignment.");
        }

        KeycloakUserDto keycloakUser = new()
        {
            Username = restaurantAdmin.UserName,
            Email = restaurantAdmin.Email,
            FirstName = restaurantAdmin.FirstName,
            LastName = restaurantAdmin.LastName,
            Enabled = true,
            Credentials = [new KeycloakCredentialDto { Type = "password", Value = restaurantAdmin.Password, Temporary = false }]
        };

        string keycloakUserId = await keycloakAdminService.CreateUserAsync(keycloakUser, cancellationToken);
        User user = User.Create(
            keycloakUserId,
            restaurantAdmin.UserName,
            restaurantAdmin.FirstName,
            restaurantAdmin.LastName,
            restaurantAdmin.Email,
            restaurantAdmin.PhoneNumber,
            customerRole);

        user.AddRole(restaurantAdminRole);

        CompanyMembership membership = CompanyMembership.CreateRestaurantAdmin(
            user.Id,
            restaurantAdmin.CompanyId,
            restaurantAdmin.RestaurantId,
            restaurantAdminRole.Id,
            restaurantAdmin.IdempotencyKey);

        identityDbContext.User.Add(user);
        identityDbContext.CompanyMembership.Add(membership);
        await identityDbContext.SaveChangesAsync(cancellationToken);

        return new RestaurantAdminRegistrationResult(membership.Id.Value, null);
    }
}
