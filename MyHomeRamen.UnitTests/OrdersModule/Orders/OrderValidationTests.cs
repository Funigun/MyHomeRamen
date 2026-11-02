using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Order;
using MyHomeRamen.Domain.Orders.Orders;
using MyHomeRamen.Domain.Orders.Products;
using MyHomeRamen.Domain.Orders.Users;

namespace MyHomeRamen.UnitTests.OrdersModule.Orders;

public class OrderValidationTests
{
    private static readonly OrderId DefaultId = new(Guid.NewGuid());
    private static readonly UserId DefaultUserId = new(Guid.NewGuid());
    private static readonly Guid DefaultRestaurantId = Guid.NewGuid();

    [Fact]
    public void CreateDineIn_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        Product product = CreateProduct();
        List<Product> products = [product];

        // Act
        Order order = Order.CreateDineIn(DefaultId, DefaultUserId, DefaultRestaurantId, "USD", products);

        // Assert
        Assert.Equal(DefaultId, order.Id);
        Assert.Equal(products, order.Products);
        Assert.Equal(OrderType.DineIn, order.Type);
        Assert.Equal(DefaultRestaurantId, order.RestaurantId);
        Assert.Equal("USD", order.Currency);
        Assert.Equal(product.FinalChargedLinePrice, order.TotalOriginalAmount);
        Assert.NotEqual(Guid.Empty, order.ReferenceNumber);
    }

    [Fact]
    public void CreateTakeOut_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        Product product = CreateProduct();
        List<Product> products = [product];

        // Act
        Order order = Order.CreateTakeOut(DefaultId, DefaultUserId, DefaultRestaurantId, "USD", products);

        // Assert
        Assert.Equal(DefaultId, order.Id);
        Assert.Equal(products, order.Products);
        Assert.Equal(OrderType.TakeOut, order.Type);
        Assert.NotEqual(Guid.Empty, order.ReferenceNumber);
    }

    [Fact]
    public void CreateDelivery_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        Product product = CreateProduct();
        List<Product> products = [product];

        // Act
        OrderAddress deliveryAddress = CreateDeliveryAddress();
        Order order = Order.CreateDelivery(DefaultId, DefaultUserId, DefaultRestaurantId, "USD", deliveryAddress, products);

        // Assert
        Assert.Equal(DefaultId, order.Id);
        Assert.Equal(products, order.Products);
        Assert.Equal(OrderType.Delivery, order.Type);
        Assert.Equal(deliveryAddress, order.DeliveryAddress);
        Assert.NotEqual(Guid.Empty, order.ReferenceNumber);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NoProductsAssigned()
    {
        // Arrange
        List<Product> products = [];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => Order.CreateDineIn(DefaultId, DefaultUserId, DefaultRestaurantId, "USD", products));
        Assert.Equal(OrderErrors.OrderMustHaveProducts().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_TooManyProducts()
    {
        // Arrange
        List<Product> products = Enumerable.Range(0, OrderConstants.MaxProductsCount + 1)
            .Select(_ => CreateProduct())
            .ToList();

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => Order.CreateDineIn(DefaultId, DefaultUserId, DefaultRestaurantId, "USD", products));
        Assert.Equal(OrderErrors.TooManyProducts().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_AmountIsTooSmallForDelivery()
    {
        // Arrange
        Product product = CreateInvalidProductForDelivery();
        List<Product> products = [product];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => Order.CreateDelivery(DefaultId, DefaultUserId, DefaultRestaurantId, "USD", CreateDeliveryAddress(), products));
        Assert.Equal(OrderErrors.DeliveryAmountTooSmall().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_AmountIsTooHigh()
    {
        // Arrange
        Product product = CreateProduct();
        int numberOfProducts = (int)(OrderConstants.MaxTotalAmount / product.OriginalPrice) + 1;
        List<Product> products = [];

        for (int i = 0; i < numberOfProducts; i++)
        {
            products.Add(product);
        }

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => Order.CreateDelivery(DefaultId, DefaultUserId, DefaultRestaurantId, "USD", CreateDeliveryAddress(), products));
        Assert.Equal(OrderErrors.AmountTooLarge().Message, exception.Message);

        exception = Assert.Throws<DomainException>(() => Order.CreateDineIn(DefaultId, DefaultUserId, DefaultRestaurantId, "USD", products));
        Assert.Equal(OrderErrors.AmountTooLarge().Message, exception.Message);

        exception = Assert.Throws<DomainException>(() => Order.CreateTakeOut(DefaultId, DefaultUserId, DefaultRestaurantId, "USD", products));
        Assert.Equal(OrderErrors.AmountTooLarge().Message, exception.Message);
    }

    private static Product CreateProduct()
    {
        return Product.Create(
            new ProductId(Guid.NewGuid()),
            new ProductId(Guid.NewGuid()),
            Guid.NewGuid(),
            "Delicious Ramen",
            OrderConstants.MinDeliveryAmount,
            1,
            OrderConstants.MinDeliveryAmount,
            [],
            []);
    }

    private static OrderAddress CreateDeliveryAddress()
        => OrderAddress.Create("Test street", "Test city", "Test building", "Test apartment", "12345");

    private static Product CreateInvalidProductForDelivery()
    {
        return Product.Create(
            new ProductId(Guid.NewGuid()),
            new ProductId(Guid.NewGuid()),
            Guid.NewGuid(),
            "Delicious Ramen",
            OrderConstants.MinDeliveryAmount - 0.1m,
            1,
            OrderConstants.MinDeliveryAmount - 0.1m,
            [],
            []);
    }
}
