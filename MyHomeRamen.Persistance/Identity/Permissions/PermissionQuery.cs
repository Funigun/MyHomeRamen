using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Identity;

public sealed partial class PermissionRepository : IPermissionQuery
{
    async Task<IReadOnlyCollection<Permission>> IPermissionQuery.All(CancellationToken cancellationToken)
        => await identityDbContext.Permissions.AsNoTracking().ToArrayAsync(cancellationToken);
     
    public async Task<IReadOnlyCollection<Permission>> ByUserId(Guid userId, CancellationToken cancellationToken)
    {
        List<RoleId> roleIds = await identityDbContext.Users.AsNoTracking()
                      .Where(user => user.Id == new UserId(userId))
                      .SelectMany(user => user.Roles.Select(role => role.Id))
                      .ToListAsync(cancellationToken);

        return await identityDbContext.RolePermissions.AsNoTracking()
                      .Where(rolePermission => roleIds.Contains(rolePermission.RoleId))
                      .Join(identityDbContext.Permissions, rolePermission => rolePermission.PermissionId, permission => permission.Id, (rolePermission, permission) => permission)
                      .Distinct()
                      .ToArrayAsync(cancellationToken);
    }
}
