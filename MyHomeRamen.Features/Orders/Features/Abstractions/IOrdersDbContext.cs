using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Orders.Features.Ingredients.Common;
using MyHomeRamen.Features.Orders.Features.Orders.Common;
using MyHomeRamen.Features.Orders.Features.Payments.Common;
using MyHomeRamen.Features.Orders.Features.Products.Common;

namespace MyHomeRamen.Features.Orders.Features.Abstractions;

public interface IOrdersDbContext : IUnitOfWork
{
    IOrderRepository Order {  get; }

    IProductRepository Product { get; }

    IIngredientRepository Ingredient { get; }

    IPaymentRepository Payment { get; }
}
