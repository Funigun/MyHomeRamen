using MyHomeRamen.Domain.Orders.Orders;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Order;
using MyHomeRamen.Features.Orders.ExternalApi;
using MyHomeRamen.Features.Orders.Features.Abstractions;
using MyHomeRamen.Features.Orders.Features.Orders.Common;
using MyHomeRamen.Features.Orders.Services;
using MyHomeRamen.Features.Restaurants.ExternalApi;
using NSubstitute;

namespace MyHomeRamen.UnitTests.OrdersModule.Orders;

public sealed class OrdersServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_ShouldPersistCheckoutSnapshots_WhenRequestIsValidated()
    {
        IOrdersDbContext dbContext = Substitute.For<IOrdersDbContext>();
        IOrderRepository orderRepository = Substitute.For<IOrderRepository>();
        dbContext.Order.Returns(orderRepository);
        dbContext.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        Guid restaurantId = Guid.NewGuid();
        IRestaurantService restaurantService = Substitute.For<IRestaurantService>();
        restaurantService.GetRestaurantAsync(restaurantId, Arg.Any<CancellationToken>())
            .Returns(new RestaurantInfo(restaurantId, Guid.NewGuid(), true));
        Guid productId = Guid.NewGuid();
        Guid offerId = Guid.NewGuid();
        CreateOrderFromCheckoutRequest request = new(
            restaurantId,
            "usd",
            Guid.NewGuid(),
            OrderFulfilmentType.TakeOut,
            [
                new OrderLineSnapshot(
                    productId,
                    offerId,
                    "Test ramen",
                    10m,
                    2,
                    22m,
                    [],
                    [])
            ],
            null);

        OrdersService service = new(dbContext, restaurantService);

        Guid orderId = await service.CreateOrderAsync(request, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, orderId);
        orderRepository.Received(1).Add(Arg.Is<Order>(order =>
            order.Id.Value == orderId
            && order.RestaurantId == restaurantId
            && order.Currency == "USD"
            && order.TotalOriginalAmount == 22m
            && order.Products.Single().OriginalId.Value == productId
            && order.Products.Single().RestaurantProductOfferId == offerId
            && order.Products.Single().Quantity == 2
            && order.Products.Single().FinalChargedLinePrice == 22m));
        await dbContext.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldRejectInactiveRestaurant_WhenCreatingOrder()
    {
        Guid restaurantId = Guid.NewGuid();
        IOrdersDbContext dbContext = Substitute.For<IOrdersDbContext>();
        IOrderRepository orderRepository = Substitute.For<IOrderRepository>();
        dbContext.Order.Returns(orderRepository);
        IRestaurantService restaurantService = Substitute.For<IRestaurantService>();
        restaurantService.GetRestaurantAsync(restaurantId, Arg.Any<CancellationToken>())
            .Returns(new RestaurantInfo(restaurantId, Guid.NewGuid(), false));
        OrdersService service = new(dbContext, restaurantService);
        CreateOrderFromCheckoutRequest request = new(
            restaurantId,
            "USD",
            Guid.NewGuid(),
            OrderFulfilmentType.TakeOut,
            [],
            null);

        DomainException exception = await Assert.ThrowsAsync<DomainException>(
            () => service.CreateOrderAsync(request, CancellationToken.None));

        Assert.Equal(OrderErrors.RestaurantUnavailable().Message, exception.Message);
        orderRepository.DidNotReceiveWithAnyArgs().Add(default!);
        await dbContext.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }
}
