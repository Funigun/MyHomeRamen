using FluentValidation;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed record AddItemToBasketCommand(AddItemToBasketRequest AddItemToBasketRequest) : ICommand<AddItemToBasketResponse>;

public sealed class AddItemToBasketValidator : AbstractValidator<AddItemToBasketCommand>
{
    public AddItemToBasketValidator(IMenuService menuService)
    {
        RuleFor(x => x.AddItemToBasketRequest.ProductId)
            .NotEmpty();

        RuleFor(x => x.AddItemToBasketRequest.Quantity)
            .MustBeValidBasketItemQuantity();

        RuleFor(x => x.AddItemToBasketRequest.BaseIngredients)
            .NotNull();

        RuleForEach(x => x.AddItemToBasketRequest.BaseIngredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(i => i.Id).NotEmpty();
                ingredient.RuleFor(i => i.Quantity).MustBeValidBasketItemQuantity();
            });

        RuleFor(x => x.AddItemToBasketRequest.CustomIngredients)
            .NotNull();

        RuleForEach(x => x.AddItemToBasketRequest.CustomIngredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(i => i.Id).NotEmpty();
                ingredient.RuleFor(i => i.Quantity).MustBeValidBasketItemQuantity();
            });

        RuleFor(x => x.AddItemToBasketRequest.Comments)
            .MustBeValidBasketItemComment();

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
                await menuService.ValidateProductConfigurationAsync(
                    cmd.AddItemToBasketRequest.ProductId,
                    cmd.AddItemToBasketRequest.BaseIngredients.Select(i => i.Id).ToList(),
                    cmd.AddItemToBasketRequest.CustomIngredients.Select(i => i.Id).ToList(),
                    ct))
            .WithMessage("Product configuration is invalid: product does not exist or the selected ingredients are not valid for this product.");
    }
}

public sealed class AddItemToBasketHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser, IMenuService menuService)
                  : ICommandHandler<AddItemToBasketCommand, AddItemToBasketResponse>
{
    public async Task<AddItemToBasketResponse> Handle(AddItemToBasketCommand command, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        Basket? basket = await dbContext.Basket.Load().GetForUserTrackedAsync(userId, cancellationToken);

        if (basket is null)
        {
            basket = Basket.Create(new BasketId(Guid.CreateVersion7()), userId);
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

internal static class Mappings
{
    internal static Product ToShoppingCartProduct(
        this MenuProductResult result,
        IEnumerable<BasketIngredientDto> baseIngredients,
        IEnumerable<BasketIngredientDto> customIngredients)
    {
        List<Ingredient> base_ = result.BaseIngredients
            .Select(i =>
            {
                int qty = baseIngredients.FirstOrDefault(r => r.Id == i.Id)?.Quantity ?? 1;
                return Ingredient.Create(
                    new IngredientId(Guid.CreateVersion7()),
                    new IngredientId(i.Id),
                    i.Name,
                    i.Description,
                    i.Price,
                    qty);
            })
            .ToList();

        List<Ingredient> custom = result.CustomIngredients
            .Select(i =>
            {
                int qty = customIngredients.FirstOrDefault(r => r.Id == i.Id)?.Quantity ?? 1;
                return Ingredient.Create(
                    new IngredientId(Guid.CreateVersion7()),
                    new IngredientId(i.Id),
                    i.Name,
                    i.Description,
                    i.Price,
                    qty);
            })
            .ToList();

        return Product.Create(
            new ProductId(Guid.CreateVersion7()),
            new ProductId(result.Id),
            result.Name,
            result.Description,
            result.Price,
            result.ImageUrl,
            base_,
            custom);
    }

    internal static BasketItem ToBasketItem(
        this Product product,
        int quantity,
        string? comment)
    {
        return BasketItem.Create(
            new BasketItemId(Guid.CreateVersion7()),
            product,
            quantity,
            comment);
    }
}
