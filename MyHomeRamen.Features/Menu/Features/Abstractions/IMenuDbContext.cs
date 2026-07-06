using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Permissions.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Menu.Features.Roles;
using MyHomeRamen.Features.Menu.Features.Users.Common;

namespace MyHomeRamen.Features.Menu.Features.Abstractions;

public interface IMenuDbContext : IMenuUnitOfWork
{
    IProductRepository Product { get; }

    ICategoryRepository Category { get; }

    IIngredientRepository Ingredient { get; }

    IUserRepository User { get; }

    IRoleRepository Role { get; }

    IPermissionRepository Permission { get; }
}
