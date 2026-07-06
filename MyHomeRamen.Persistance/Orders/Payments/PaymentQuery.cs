using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Orders.Payments;
using MyHomeRamen.Features.Orders.Features.Payments.Common;

namespace MyHomeRamen.Persistance.Orders;

public partial class OrdersDbContext : IPaymentQuery
{

}
