using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Orders.Roles;
using MyHomeRamen.Features.Orders.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Orders;

public partial class OrdersDbContext : IRoleQuery
{

}
