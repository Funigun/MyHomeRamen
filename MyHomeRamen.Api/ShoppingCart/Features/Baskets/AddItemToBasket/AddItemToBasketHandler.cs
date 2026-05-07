using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket.Models;
using MyHomeRamen.Common.Contracts.Menu;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed class AddItemToBasketHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser, IMenuService menuService)
                  : IRequestHandler<AddItemToBasketRequest, AddItemToBasketResponse>
{
    public async Task<AddItemToBasketResponse> Handle(AddItemToBasketRequest request, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        User user = await dbContext.Users.FirstAsync(user => user.Id == userId, cancellationToken);

        Basket basket = await dbContext.ShoppingCarts.ForUserTracked(userId).FirstAsync(cancellationToken);

        if (basket is null)
        {
            basket = Basket.Create(new BasketId(Guid.CreateVersion7()), user);
            dbContext.ShoppingCarts.Add(basket);
        }

        MenuProductResult menuProduct = (await menuService.GetProductWithSelectedIngredientsAsync(
            request.ProductId,
            request.BaseIngredients.Select(i => i.Id).ToList(),
            request.CustomIngredients.Select(i => i.Id).ToList(),
            cancellationToken))!;

        Product product = menuProduct.ToShoppingCartProduct(request.BaseIngredients, request.CustomIngredients);

        dbContext.Products.Add(product);
        dbContext.Ingredients.AddRange(product.BaseIngredients);
        dbContext.Ingredients.AddRange(product.CustomIngredients);

        Domain.ShoppingCart.BasketItems.BasketItem basketItem = product.ToBasketItem(request.Quantity, product.TotalPrice, request.Comments);

        basket.AddItem(basketItem);
        dbContext.BasketItems.Add(basketItem);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddItemToBasketResponse(basket.Id.Value, basketItem.Id.Value);
    }
}
