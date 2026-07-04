using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Menu.Features.Users.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IPermissionSpecification
{
    public async Task<Permission> ById(PermissionId permissionId, CancellationToken cancellationToken)
        => await Permissions.FirstAsync(permission => permission.Id == permissionId, cancellationToken);
}
