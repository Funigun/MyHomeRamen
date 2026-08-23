using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Features.Identity.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Identity;

public sealed partial class PermissionRepository : IPermissionLoader
{
    public async Task<IEnumerable<Permission>> All(CancellationToken cancellationToken)
        => await identityDbContext.Permissions.ToListAsync(cancellationToken);

    public Task<Permission?> ByModuleAndName(string module, string name, CancellationToken cancellationToken)
        => identityDbContext.Permissions.FirstOrDefaultAsync(
            permission => permission.Module == module && permission.Name == name,
            cancellationToken);
}
