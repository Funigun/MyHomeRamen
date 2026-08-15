using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Features.Menu.Features.Abstractions;

public interface IMenuDbContext : IUnitOfWork
{
    IProductRepository Product { get; }

    ICategoryRepository Category { get; }

    IIngredientRepository Ingredient { get; }
}
