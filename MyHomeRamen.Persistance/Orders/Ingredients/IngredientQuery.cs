using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Orders.Ingredients;
using MyHomeRamen.Features.Orders.Features.Ingredients.Common;

namespace MyHomeRamen.Persistance.Orders;

public partial class OrdersDbContext : IIngredientQuery
{

}
