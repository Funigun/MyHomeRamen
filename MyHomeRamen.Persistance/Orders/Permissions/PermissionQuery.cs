using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Orders.Permissions;
using MyHomeRamen.Features.Orders.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Orders;

public partial class OrdersDbContext : IPermissionQuery
{

}
