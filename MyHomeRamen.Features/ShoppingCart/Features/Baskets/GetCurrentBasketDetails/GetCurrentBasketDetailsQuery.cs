using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;

public sealed record GetCurrentBasketDetailsQuery : IQuery<GetCurrentBasketDetailsResponse?>;

public sealed record GetCurrentBasketDetailsQueryOptions(UserId UserId)
    : DbQueryOptions<Basket, CurrentBasketDetailsDto>
    (
        new()
        {
            Filter = basket => basket.User.Id == UserId && basket.Status == BasketStatus.Active,
            Selector = basket => new CurrentBasketDetailsDto(
                basket.Id.Value,
                basket.Items.Select(item => new BasketDetailsItemDto(
                    item.Id.Value,
                    item.Quantity,
                    item.Price,
                    item.Comment,
                    new BasketDetailsItemProductDto(
                        item.Product.Id.Value,
                        item.Product.Name,
                        item.Product.Description,
                        item.Product.ImageUrl,
                        item.Product.BaseIngredients.Select(ingredient => new BasketDetailsIngredientDto(ingredient.Id.Value, ingredient.Name, ingredient.Description, ingredient.Price)),
                        item.Product.CustomIngredients.Select(ingredient => new BasketDetailsIngredientDto(ingredient.Id.Value, ingredient.Name, ingredient.Description, ingredient.Price))))))
        }
    );

public sealed class GetCurrentBasketDetailsHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    : IQueryHandler<GetCurrentBasketDetailsQuery, GetCurrentBasketDetailsResponse?>
{
    public async Task<GetCurrentBasketDetailsResponse?> Handle(GetCurrentBasketDetailsQuery request, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        CurrentBasketDetailsDto? basket = await dbContext.Basket.Query().GetCurrentBasketDetailsAsync(new GetCurrentBasketDetailsQueryOptions(userId), cancellationToken);

        return basket is null
            ? null
            : new GetCurrentBasketDetailsResponse(basket.BasketId, basket.Items);
    }
}

internal static class Mappings
{
    public static GetCurrentBasketDetailsResponse ToResponse(this Basket basket)
        => new(
            basket.Id.Value,
            basket.Items.Select(i => i.ToDto()));

    private static BasketDetailsItemDto ToDto(this BasketItem item)
        => new(
            item.Id.Value,
            item.Quantity,
            item.Price,
            item.Comment,
            item.Product.ToProductDto());

    private static BasketDetailsItemProductDto ToProductDto(this Product product)
        => new(
            product.Id.Value,
            product.Name,
            product.Description,
            product.ImageUrl,
            product.BaseIngredients.Select(i => new BasketDetailsIngredientDto(i.Id.Value, i.Name, i.Description, i.Price)),
            product.CustomIngredients.Select(i => new BasketDetailsIngredientDto(i.Id.Value, i.Name, i.Description, i.Price)));
}

