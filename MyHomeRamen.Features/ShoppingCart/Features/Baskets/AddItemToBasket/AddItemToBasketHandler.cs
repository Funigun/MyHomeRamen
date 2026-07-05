using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed class AddItemToBasketHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser, IMenuService menuService)
                  : ICommandHandler<AddItemToBasketCommand, AddItemToBasketResponse>
{
    public async Task<AddItemToBasketResponse> Handle(AddItemToBasketCommand command, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        User? user = await dbContext.User.Query().FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException($"User {userId.Value} was not found.");
        }

        Basket? basket = await dbContext.Basket.Specification().GetForUserTrackedAsync(userId, cancellationToken);

        if (basket is null)
        {
            basket = Basket.Create(new BasketId(Guid.CreateVersion7()), user);
            dbContext.Basket.Add(basket);
        }

        MenuProductResult menuProduct = (await menuService.GetProductWithSelectedIngredientsAsync(
            command.AddItemToBasketRequest.ProductId,
            command.AddItemToBasketRequest.BaseIngredients.Select(i => i.Id).ToList(),
            command.AddItemToBasketRequest.CustomIngredients.Select(i => i.Id).ToList(),
            cancellationToken))!;

        Product product = menuProduct.ToShoppingCartProduct(command.AddItemToBasketRequest.BaseIngredients, command.AddItemToBasketRequest.CustomIngredients);

        dbContext.Product.Add(product);
        dbContext.Ingredient.AddRange(product.BaseIngredients);
        dbContext.Ingredient.AddRange(product.CustomIngredients);

        BasketItem basketItem = product.ToBasketItem(command.AddItemToBasketRequest.Quantity, command.AddItemToBasketRequest.Comments);

        basket.AddItem(basketItem);
        dbContext.BasketItem.Add(basketItem);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddItemToBasketResponse(basket.Id.Value, basketItem.Id.Value);
    }
}

