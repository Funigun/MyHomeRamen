using FluentValidation;
using FluentValidation.Results;
using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Features.Identity.Services;

public sealed class IdentityService(
    IKeycloakAdminService keycloakAdminService,
    IIdentityDbContext identityDbContext,
    IValidator<CompanyOwnerDto> companyOwnerValidator) : IIdentityService
{
    public async Task<CompanyOwnerRegistrationResult> RegisterCompanyOwnerAsync(CompanyOwnerDto companyOwner, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await companyOwnerValidator.ValidateAsync(companyOwner, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            string error = string.Join(" ", validationResult.Errors.Select(validationError => validationError.ErrorMessage));
            return new CompanyOwnerRegistrationResult(null, error);
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
        
        User user = User.Create(keycloakUserId, companyOwner.UserName, companyOwner.FirstName, companyOwner.LastName, companyOwner.Email, companyOwner.PhoneNumber);
        user.AssignRole(customerRole);
        user.AssignRole(ownerRole);

        identityDbContext.User.Add(user);
        await identityDbContext.SaveChangesAsync(cancellationToken);

        return new CompanyOwnerRegistrationResult(user.Id.Value, null);
    }

    public Task<bool> IsCompanyOwnerConfiguredAsync(CancellationToken cancellationToken)
        => identityDbContext.User.Query().IsCompanyOwnerConfigured(cancellationToken);

    public async Task<Guid> CreateRestaurantAdminRoleAsync(Guid restaurantId, CancellationToken cancellationToken)
    {
        if (restaurantId == Guid.Empty)
        {
            throw new ArgumentException("Restaurant identifier must not be empty.", nameof(restaurantId));
        }

        IEnumerable<Permission> permissions = await identityDbContext.Permission.Load().All(cancellationToken);
        IEnumerable<PermissionId> permissionIds = permissions
            .Where(permission => PermissionCatalog.RestaurantAdminPermissions.Contains((permission.Module, permission.Name)))
            .Select(permission => permission.Id);

        Role? role = await identityDbContext.Role.Load()
            .ByName(RoleConstants.RestaurantAdmin, cancellationToken, restaurantId);
        if (role is null)
        {
            role = Role.CreateAdmin(permissionIds, restaurantId);
            identityDbContext.Role.Add(role);
        }
        else
        {
            role.UpdatePermissions(permissionIds);
        }

        await identityDbContext.SaveChangesAsync(cancellationToken);
        return role.Id.Value;
    }
}
