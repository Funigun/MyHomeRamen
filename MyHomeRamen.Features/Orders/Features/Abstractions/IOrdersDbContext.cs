using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Orders.Features.Ingredients.Common;
using MyHomeRamen.Features.Orders.Features.Orders.Common;
using MyHomeRamen.Features.Orders.Features.Payments.Common;
using MyHomeRamen.Features.Orders.Features.Permissions.Common;
using MyHomeRamen.Features.Orders.Features.Products.Common;
using MyHomeRamen.Features.Orders.Features.Roles.Common;
using MyHomeRamen.Features.Orders.Features.Users.Common;

namespace MyHomeRamen.Features.Orders.Features.Abstractions;

public interface IOrdersDbContext : IUnitOfWork
{
    IOrderRepository Order {  get; }

    IProductRepository Product { get; }

    IIngredientRepository Ingredient { get; }

    IPaymentRepository Payment { get; }

    IPermissionRepository Permission { get; }

    IRoleRepository Role { get; }

    IUserRepository User { get; }
}
