using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Ingredients.Common;
using MyHomeRamen.Features.ShoppingCart.Features.Products.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

public interface IShoppingCartDbContext : IUnitOfWork
{
    IBasketRepository Basket { get; }

    IBasketItemRepository BasketItem { get; }

    IProductRepository Product { get; }

    IIngredientRepository Ingredient { get; }
}
