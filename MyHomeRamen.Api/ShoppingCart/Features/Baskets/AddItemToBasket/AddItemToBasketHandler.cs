using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed class AddItemToBasketHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser, IMenuService menuService)
                  : IRequestHandler<AddItemToBasketCommand, AddItemToBasketResponse>
{
    public async Task<AddItemToBasketResponse> Handle(AddItemToBasketCommand command, CancellationToken cancellationToken)
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
            command.AddItemToBasketRequest.ProductId,
            command.AddItemToBasketRequest.BaseIngredients.Select(i => i.Id).ToList(),
            command.AddItemToBasketRequest.CustomIngredients.Select(i => i.Id).ToList(),
            cancellationToken))!;

        Product product = menuProduct.ToShoppingCartProduct(command.AddItemToBasketRequest.BaseIngredients, command.AddItemToBasketRequest.CustomIngredients);

        dbContext.Products.Add(product);
        dbContext.Ingredients.AddRange(product.BaseIngredients);
        dbContext.Ingredients.AddRange(product.CustomIngredients);

        Domain.ShoppingCart.BasketItems.BasketItem basketItem = product.ToBasketItem(command.AddItemToBasketRequest.Quantity, product.TotalPrice, command.AddItemToBasketRequest.Comments);

        basket.AddItem(basketItem);
        dbContext.BasketItems.Add(basketItem);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddItemToBasketResponse(basket.Id.Value, basketItem.Id.Value);
    }
}
