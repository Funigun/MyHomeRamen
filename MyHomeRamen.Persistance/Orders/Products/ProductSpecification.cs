using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Orders.Products;
using MyHomeRamen.Features.Orders.Features.Products.Common;

namespace MyHomeRamen.Persistance.Orders;

public partial class OrdersDbContext : IProductSpecification
{

}
