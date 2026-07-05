using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Roles.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IRoleSpecification
{
    public async Task<Role?> ByIdAsync(RoleId roleId, CancellationToken cancellationToken)
        => await Roles.Include(role => role.Permissions).FirstOrDefaultAsync(role => role.Id == roleId, cancellationToken);
}
