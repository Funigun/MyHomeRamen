using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Roles;
using MyHomeRamen.Features.ShoppingCart.Features.Roles.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class RoleRepository : IRoleSpecification
{
    public async Task<Role?> ByIdAsync(RoleId roleId, CancellationToken cancellationToken)
        => await shoppingCartDbContext.Roles.Include(role => role.Permissions).FirstOrDefaultAsync(role => role.Id == roleId, cancellationToken);

    public async Task<IEnumerable<Role>> GetAllWithPermissions(CancellationToken cancellationToken)
        => await shoppingCartDbContext.Roles.Include(role => role.Permissions).ToListAsync(cancellationToken);
}
