using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Permissions;

namespace MyHomeRamen.Features.Identity.Services;

public sealed class PermissionCatalogSynchronizer(IIdentityDbContext identityDbContext) : IPermissionCatalogSynchronizer
{
    public async Task Synchronize(CancellationToken cancellationToken)
    {
        IEnumerable<Permission> existingPermissions = await identityDbContext.Permission.Load().All(cancellationToken);
        
        Dictionary<(string Module, string Name), Permission> existingPermissionsByKey = existingPermissions.ToDictionary(permission => (permission.Module, permission.Name));
        
        HashSet<string> modules = PermissionCatalog.Definitions.Select(definition => definition.Module)
                                                               .ToHashSet(StringComparer.Ordinal);

        IEnumerable<Permission> addedPermissions = AddMissingPermissions(existingPermissionsByKey);
        IEnumerable<Permission> removedPermissions = RemoveOldPermissions(existingPermissions, modules);

        await identityDbContext.SaveChangesAsync(cancellationToken);

        IEnumerable<Permission> allCurrentPermissions = existingPermissions.Except(removedPermissions).Concat(addedPermissions);
        
        await UpdateAdminRole(allCurrentPermissions, cancellationToken);
        await UpdateGuestRole(allCurrentPermissions, cancellationToken);
        await UpdateCustomerRole(allCurrentPermissions, cancellationToken);

        await identityDbContext.SaveChangesAsync(cancellationToken);
    }

    private List<Permission> AddMissingPermissions(Dictionary<(string Module, string Name), Permission> existingPermissionsByKey)
    {
        List<Permission> permissions = [];

        foreach (PermissionDefinition definition in PermissionCatalog.Definitions)
        {
            (string Module, string Name) key = (definition.Module, definition.Name);

            if (!existingPermissionsByKey.ContainsKey(key))
            {
                Permission permission = Permission.Create(definition.Name, definition.Description, definition.Module);
                permissions.Add(permission);
                identityDbContext.Permission.Add(permission);
            }
        }

        return permissions;
    }

    private IEnumerable<Permission> RemoveOldPermissions(IEnumerable<Permission> existingPermissions, HashSet<string> modules)
    {
        HashSet<(string Module, string Name)> definedKeys = PermissionCatalog.Definitions
                                                                             .Select(definition => (definition.Module, definition.Name))
                                                                             .ToHashSet();

        IEnumerable<Permission> removedPermissions = existingPermissions.Where(permission => modules.Contains(permission.Module) && !definedKeys.Contains((permission.Module, permission.Name)));

        foreach (Permission permission in removedPermissions)
        {
            identityDbContext.Permission.Delete(permission);
        }

        return removedPermissions;
    }

    private async Task UpdateAdminRole(IEnumerable<Permission> allPermissions, CancellationToken cancellationToken)
    {
        IEnumerable<PermissionId> allPermissionIds = allPermissions.Select(p => p.Id);

        Role? adminRole = await identityDbContext.Role.Load().ByName(RoleConstants.Admin, cancellationToken);

        if (adminRole is null)
        {
            identityDbContext.Role.Add(Role.CreateAdmin(allPermissionIds));
        }
        else
        {
            adminRole.UpdatePermissions(allPermissionIds);
        }
    }

    private async Task UpdateGuestRole(IEnumerable<Permission> allCurrentPermissions, CancellationToken cancellationToken)
    {
        IEnumerable<PermissionId> guestPermissionIds = allCurrentPermissions.Where(permission => PermissionCatalog.GuestPermissions.Contains((permission.Module, permission.Name)))
                                                                            .Select(p => p.Id);

        Role? guestRole = await identityDbContext.Role.Load().ByName(RoleConstants.Guest, cancellationToken);

        if (guestRole is null)
        {
            identityDbContext.Role.Add(Role.CreateGuest(guestPermissionIds));
        }
        else
        {
            guestRole.UpdatePermissions(guestPermissionIds);
        }
    }

    private async Task UpdateCustomerRole(IEnumerable<Permission> allCurrentPermissions, CancellationToken cancellationToken)
    {
        HashSet<(string Module, string Name)> customerPermissions = PermissionCatalog.GuestPermissions
                                                                                     .Append(("Identity", IdentityPermissionConstants.CanViewUserProfile))
                                                                                     .Append(("Identity", IdentityPermissionConstants.CanUpdateUserProfile))
                                                                                     .Append(("Identity", IdentityPermissionConstants.CanDeleteUserProfile))
                                                                                     .ToHashSet();

        IEnumerable<PermissionId> customerPermissionIds = allCurrentPermissions.Where(permission => customerPermissions.Contains((permission.Module, permission.Name)))
                                                                                .Select(permission => permission.Id);

        Role? customerRole = await identityDbContext.Role.Load().ByName(RoleConstants.Customer, cancellationToken);

        if (customerRole is null)
        {
            identityDbContext.Role.Add(Role.CreateCustomer(customerPermissionIds));
        }
        else
        {
            customerRole.UpdatePermissions(customerPermissionIds);
        }
    }
}
