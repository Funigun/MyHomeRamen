using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Menu.ExternalApi;
using MyHomeRamen.Features.Orders.ExternalApi;
using MyHomeRamen.Features.Restaurants.ExternalApi;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.CheckoutBasket;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;
using NSubstitute;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Baskets;

public sealed class CheckoutBasketTests
{
    [Fact]
    public async Task Handle_ShouldRejectRestaurantDifferentFromBasket_WhenCheckoutRequested()
    {
        UserId userId = new(Guid.NewGuid());
        Guid basketRestaurantId = Guid.NewGuid();
        Basket basket = Basket.Create(new BasketId(Guid.NewGuid()), userId, basketRestaurantId);
        ICurrentUser currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(userId.Value);

        IShoppingCartDbContext dbContext = Substitute.For<IShoppingCartDbContext>();
        IBasketRepository basketRepository = Substitute.For<IBasketRepository>();
        IBasketLoader basketLoader = Substitute.For<IBasketLoader>();
        dbContext.Basket.Returns(basketRepository);
        basketRepository.Load().Returns(basketLoader);
        basketLoader.GetForUserWithCheckoutTrackedAsync(userId, Arg.Any<CancellationToken>()).Returns(basket);

        IRestaurantService restaurantService = Substitute.For<IRestaurantService>();
        IMenuService menuService = Substitute.For<IMenuService>();
        IOrdersService ordersService = Substitute.For<IOrdersService>();
        CheckoutBasketHandler handler = new(dbContext, currentUser, restaurantService, menuService, ordersService);

        DomainException exception = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(
                new CheckoutBasketCommand(new CheckoutBasketRequest(Guid.NewGuid(), "USD")),
                CancellationToken.None));

        Assert.Equal(BasketErrors.BasketRestaurantMismatch().Message, exception.Message);
        await restaurantService.DidNotReceiveWithAnyArgs().GetRestaurantAsync(default, default);
        await ordersService.DidNotReceiveWithAnyArgs().CreateOrderAsync(default!, default);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrderWithCurrentPriceAndBasketRestaurant_WhenCheckoutRequested()
    {
        UserId userId = new(Guid.NewGuid());
        Guid restaurantId = Guid.NewGuid();
        Guid offerId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid extraIngredientId = Guid.NewGuid();
        Basket basket = Basket.Create(new BasketId(Guid.NewGuid()), userId, restaurantId);
        Ingredient selectedExtra = Ingredient.Create(
            new IngredientId(Guid.NewGuid()),
            new IngredientId(extraIngredientId),
            "Extra",
            "Valid test ingredient description.",
            1m,
            1);
        Product product = Product.Create(
            new ProductId(Guid.NewGuid()),
            new ProductId(productId),
            "Old ramen name",
            "Test Desc with valid description length matching domain requirements",
            6m,
            "url",
            [],
            [selectedExtra]);
        basket.AddItem(BasketItem.Create(
            new BasketItemId(Guid.NewGuid()),
            offerId,
            product,
            2,
            null));
        basket.UpdateShippingDetails(MyHomeRamen.Domain.ShoppingCart.ShippingDetails.ShippingDetails.CreatePersonalPickup());

        ICurrentUser currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(userId.Value);
        IShoppingCartDbContext dbContext = Substitute.For<IShoppingCartDbContext>();
        dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        IBasketRepository basketRepository = Substitute.For<IBasketRepository>();
        IBasketLoader basketLoader = Substitute.For<IBasketLoader>();
        dbContext.Basket.Returns(basketRepository);
        basketRepository.Load().Returns(basketLoader);
        basketLoader.GetForUserWithCheckoutTrackedAsync(userId, Arg.Any<CancellationToken>()).Returns(basket);

        IRestaurantService restaurantService = Substitute.For<IRestaurantService>();
        restaurantService.GetRestaurantAsync(restaurantId, Arg.Any<CancellationToken>())
            .Returns(new RestaurantInfo(restaurantId, Guid.NewGuid(), true));
        IMenuService menuService = Substitute.For<IMenuService>();
        menuService.GetOfferForBasketAsync(restaurantId, offerId, Arg.Any<CancellationToken>())
            .Returns(new MenuOfferResult(
                restaurantId,
                offerId,
                productId,
                "Current ramen name",
                "Current product description",
                10m,
                "current-url",
                [],
                [new MenuOfferIngredientResult(extraIngredientId, "Current extra", 3m)]));
        IOrdersService ordersService = Substitute.For<IOrdersService>();
        Guid orderId = Guid.NewGuid();
        ordersService.CreateOrderAsync(Arg.Any<CreateOrderFromCheckoutRequest>(), Arg.Any<CancellationToken>())
            .Returns(orderId);
        CheckoutBasketHandler handler = new(dbContext, currentUser, restaurantService, menuService, ordersService);

        CheckoutBasketResponse? response = await handler.Handle(
            new CheckoutBasketCommand(new CheckoutBasketRequest(restaurantId, "USD")),
            CancellationToken.None);

        Assert.NotNull(response);
        Assert.Equal(orderId, response.OrderId);
        Assert.Equal(restaurantId, response.RestaurantId);
        Assert.Equal(BasketStatus.CheckedOut, basket.Status);
        await ordersService.Received(1).CreateOrderAsync(
            Arg.Is<CreateOrderFromCheckoutRequest>(request =>
                request.RestaurantId == restaurantId
                && request.UserId == userId.Value
                && request.Currency == "USD"
                && request.Type == OrderFulfilmentType.TakeOut
                && request.Lines.Count == 1
                && request.Lines[0].ProductId == productId
                && request.Lines[0].RestaurantProductOfferId == offerId
                && request.Lines[0].Name == "Current ramen name"
                && request.Lines[0].FinalChargedLinePrice == 26m),
            CancellationToken.None);
    }
}
