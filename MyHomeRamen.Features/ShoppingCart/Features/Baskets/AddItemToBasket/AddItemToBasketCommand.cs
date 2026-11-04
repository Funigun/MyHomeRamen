using FluentValidation;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.Features.Restaurants.ExternalApi;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed record AddItemToBasketCommand(AddItemToBasketRequest AddItemToBasketRequest) : ICommand<AddItemToBasketResponse>;

public sealed class AddItemToBasketAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<AddItemToBasketCommand>
{
    public async Task<bool> Authorize(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(currentUser.CanManageBasket());
    }
}

public sealed class AddItemToBasketValidator : AbstractValidator<AddItemToBasketCommand>
{
    public AddItemToBasketValidator(IMenuService menuService, ICompanyContext companyContext)
    {
        RuleFor(x => x.AddItemToBasketRequest.RestaurantId)
            .NotEmpty()
            .Must(restaurantId => restaurantId == companyContext.RestaurantId)
            .WithMessage("RestaurantId must match the X-Restaurant-Id header.");

        RuleFor(x => x.AddItemToBasketRequest.OfferId)
            .NotEmpty();

        RuleFor(x => x.AddItemToBasketRequest.Quantity)
            .MustBeValidBasketItemQuantity();

        RuleFor(x => x.AddItemToBasketRequest.ExtraIngredientIds)
            .NotNull();

        RuleForEach(x => x.AddItemToBasketRequest.ExtraIngredientIds)
            .NotEmpty();

        RuleFor(x => x.AddItemToBasketRequest.ExtraIngredientIds)
            .Must(ids => ids is null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Selected extras must be unique.");

        RuleFor(x => x.AddItemToBasketRequest.Comments)
            .MustBeValidBasketItemComment();

        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
            {
                AddItemToBasketRequest request = cmd.AddItemToBasketRequest;
                MenuOfferResult? offer = await menuService.GetOfferForBasketAsync(request.RestaurantId, request.OfferId, ct);
                return offer is not null
                    && request.ExtraIngredientIds is not null
                    && request.ExtraIngredientIds.All(id => offer.AvailableExtras.Any(extra => extra.Id == id));
            })
            .WithMessage("Restaurant offer is unavailable or one or more selected extras are not offered.");
    }
}

public sealed class AddItemToBasketHandler(
    IShoppingCartDbContext dbContext,
    ICurrentUser currentUser,
    IMenuService menuService,
    IRestaurantService restaurantService)
                  : IRequestHandler<AddItemToBasketCommand, AddItemToBasketResponse>
{
    public async Task<AddItemToBasketResponse> Handle(AddItemToBasketCommand command, CancellationToken cancellationToken)
    {
        AddItemToBasketRequest request = command.AddItemToBasketRequest;
        RestaurantInfo? restaurant = await restaurantService.GetRestaurantAsync(request.RestaurantId, cancellationToken);
        if (restaurant is not { IsActive: true })
        {
            throw BasketErrors.RestaurantUnavailable();
        }

        MenuOfferResult? menuOffer = await menuService.GetOfferForBasketAsync(
            request.RestaurantId,
            request.OfferId,
            cancellationToken);
        if (menuOffer is null)
        {
            throw BasketErrors.OfferUnavailable();
        }

        UserId userId = new(currentUser.UserId);
        Basket? basket = await dbContext.Basket.Load().GetForUserTrackedAsync(userId, cancellationToken);

        if (basket is not null && basket.RestaurantId != restaurant.Id && !request.ReplaceExistingBasket)
        {
            return new AddItemToBasketResponse(
                basket.Id.Value,
                Guid.Empty,
                RequiresBasketReplacement: true,
                CurrentRestaurantId: basket.RestaurantId);
        }

        if (basket is not null && basket.RestaurantId != restaurant.Id)
        {
            basket.Abandon();
            basket = null;
        }

        if (basket is null)
        {
            basket = Basket.Create(new BasketId(Guid.CreateVersion7()), userId, restaurant.Id);
            dbContext.Basket.Add(basket);
        }

        Product product = menuOffer.ToShoppingCartProduct(request.ExtraIngredientIds);

        dbContext.Product.Add(product);
        dbContext.Ingredient.AddRange(product.BaseIngredients);
        dbContext.Ingredient.AddRange(product.CustomIngredients);

        BasketItem basketItem = product.ToBasketItem(menuOffer.OfferId, request.Quantity, request.Comments);

        basket.AddItem(basketItem);
        dbContext.BasketItem.Add(basketItem);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddItemToBasketResponse(basket.Id.Value, basketItem.Id.Value);
    }
}

internal static class Mappings
{
    internal static Product ToShoppingCartProduct(this MenuOfferResult result, IReadOnlyCollection<Guid> selectedExtraIngredientIds)
    {
        List<Ingredient> baseIngredients = result.BaseIngredients
            .Select(ingredient => Ingredient.Create(
                new IngredientId(Guid.CreateVersion7()),
                new IngredientId(ingredient.Id),
                ingredient.Name,
                ingredient.Description,
                0,
                1))
            .ToList();

        List<Ingredient> customIngredients = result.AvailableExtras
            .Where(ingredient => selectedExtraIngredientIds.Contains(ingredient.Id))
            .Select(ingredient => Ingredient.Create(
                new IngredientId(Guid.CreateVersion7()),
                new IngredientId(ingredient.Id),
                ingredient.Name,
                ingredient.Description,
                ingredient.Price,
                1))
            .ToList();

        if (customIngredients.Count != selectedExtraIngredientIds.Count)
        {
            throw BasketErrors.OfferUnavailable();
        }

        return Product.Create(
            new ProductId(Guid.CreateVersion7()),
            new ProductId(result.ProductId),
            result.Name,
            result.Description,
            result.Price,
            result.ImageUrl,
            baseIngredients,
            customIngredients);
    }

    internal static BasketItem ToBasketItem(
        this Product product,
        Guid offerId,
        int quantity,
        string? comment)
    {
        return BasketItem.Create(
            new BasketItemId(Guid.CreateVersion7()),
            offerId,
            product,
            quantity,
            comment);
    }
}
