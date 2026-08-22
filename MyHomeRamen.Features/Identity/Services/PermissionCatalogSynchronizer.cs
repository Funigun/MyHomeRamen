using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.ExternalApi;

namespace MyHomeRamen.Features.Identity.Services;

public sealed class PermissionCatalogSynchronizer(IIdentityDbContext identityDbContext, IEnumerable<IPermissionDefinitionProvider> permissionDefinitionProviders) 
                  : IPermissionCatalogSynchronizer
{
    public async Task Synchronize(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Permission> existingPermissions = await identityDbContext.Permission.Query().All(cancellationToken);
        
        Dictionary<(string Module, string Name), Permission> existingPermissionsByKey = existingPermissions.ToDictionary(permission => (permission.Module, permission.Name));
        
        HashSet<string> modules = permissionDefinitionProviders.Select(provider => provider.ModuleName)
                                                               .ToHashSet(StringComparer.Ordinal);

        IEnumerable<Permission> addedPermissions = AddMissingPermissions(existingPermissionsByKey);
        IEnumerable<Permission> removedPermissions = RemoveOldPermissions(existingPermissions, modules, addedPermissions);

        await identityDbContext.SaveChangesAsync(cancellationToken);

        IEnumerable<Permission> allCurrentPermissions = existingPermissions.Except(removedPermissions).Concat(addedPermissions);
        
        await UpdateAdminRole(allCurrentPermissions, cancellationToken);

        await UpdateGuestRole(allCurrentPermissions, permissionDefinitionProviders, cancellationToken);

        await identityDbContext.SaveChangesAsync(cancellationToken);
    }

    private IEnumerable<Permission> AddMissingPermissions(Dictionary<(string Module, string Name), Permission> existingPermissionsByKey)
    {
        List<Permission> permissions = [];

        foreach (IPermissionDefinitionProvider provider in permissionDefinitionProviders)
        {
            foreach (PermissionDefinition definition in provider.Permissions)
            {
                (string Module, string Name) key = (provider.ModuleName, definition.Name);

                if (!existingPermissionsByKey.ContainsKey(key))
                {
                    Permission permission = Permission.Create(definition.Name, definition.Description, provider.ModuleName);
                    permissions.Add(permission);
                    identityDbContext.Permission.Add(permission);
                }
            }
        }

        return permissions;
    }

    private IEnumerable<Permission> RemoveOldPermissions(IEnumerable<Permission> existingPermissions, HashSet<string> modules, IEnumerable<Permission> addedPermissions)
    {
        HashSet<(string Module, string Name)> definedKeys = addedPermissions.Select(p => (p.Module, p.Name)).ToHashSet();

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

        Role adminRole = await identityDbContext.Role.Load().ByName(RoleConstants.Admin, cancellationToken) ?? Role.CreateAdmin(allPermissionIds);

        if (adminRole.Id.Value == Guid.Empty)
        {
            identityDbContext.Role.Add(adminRole);
        }
        else
        {
            adminRole.UpdatePermissions(allPermissionIds);
        }
    }

    private async Task UpdateGuestRole(IEnumerable<Permission> allCurrentPermissions, IEnumerable<IPermissionDefinitionProvider> permissionDefinitionProviders, CancellationToken cancellationToken)
    {
        IEnumerable<(string Module, string Name)> guestPermissions = permissionDefinitionProviders.SelectMany(provider => provider.GuestPermissions
                                                                                            .Select(p => (provider.ModuleName, p.Name)));

        IEnumerable<PermissionId> guestPermissionIds = allCurrentPermissions.Where(p => guestPermissions.Any(gp => gp.Name == p.Name && gp.Module == p.Module))
                                                                            .Select(p => p.Id);

        Role guestRole = await identityDbContext.Role.Load().ByName(RoleConstants.Guest, cancellationToken) ?? Role.CreateGuest(guestPermissionIds);

        if (guestRole.Id.Value == Guid.Empty)
        {
            identityDbContext.Role.Add(guestRole);
        }
        else
        {
            guestRole.UpdatePermissions(guestPermissionIds);
        }
    }
}
