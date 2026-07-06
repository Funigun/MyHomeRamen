using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Orders.Orders;
using MyHomeRamen.Features.Orders.Features.Orders.Common;

namespace MyHomeRamen.Persistance.Orders;

public partial class OrdersDbContext : IOrderQuery
{

}
