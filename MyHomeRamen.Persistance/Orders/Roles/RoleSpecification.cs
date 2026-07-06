using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Orders.Roles;
using MyHomeRamen.Features.Orders.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Orders;

public partial class OrdersDbContext : IRoleSpecification
{
    public Task<Role?> ByName(string orderRoleName, CancellationToken cancellationToken)
        => Roles.Where(role => role.Name == orderRoleName).FirstOrDefaultAsync(cancellationToken);
}
