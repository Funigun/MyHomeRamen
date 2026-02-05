using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Order;
using MyHomeRamen.Domain.Orders;
using MyHomeRamen.Domain.Orders.Orders;
using MyHomeRamen.Domain.Orders.Products;

namespace MyHomeRamen.UnitTests.OrdersModule.Orders;

public class OrderValidationTests
{
    private static readonly OrderId DefaultId = new(Guid.NewGuid());
    private static readonly CustomerId DefaultCustomerId = new(Guid.NewGuid());
    private static readonly PaymentId DefaultPaymentId = new(Guid.NewGuid());

    [Fact]
    public void CreateDineIn_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        Product product = CreateProduct();
        List<Product> products = [product];

        // Act
        Order order = Order.CreateDineIn(DefaultId, DefaultCustomerId, DefaultPaymentId, products);

        // Assert
        Assert.Equal(DefaultId, order.Id);
        Assert.Equal(DefaultCustomerId, order.CustomerId);
        Assert.Equal(DefaultPaymentId, order.PaymentId);
        Assert.Equal(products, order.ProductId);
        Assert.Equal(OrderType.DineIn, order.OrderType);
        Assert.NotEqual(Guid.Empty, order.ReferenceNumber);
    }

    [Fact]
    public void CreateTakeOut_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        Product product = CreateProduct();
        List<Product> products = [product];

        // Act
        Order order = Order.CreateTakeOut(DefaultId, DefaultCustomerId, DefaultPaymentId, products);

        // Assert
        Assert.Equal(DefaultId, order.Id);
        Assert.Equal(DefaultCustomerId, order.CustomerId);
        Assert.Equal(DefaultPaymentId, order.PaymentId);
        Assert.Equal(products, order.ProductId);
        Assert.Equal(OrderType.TakeOut, order.OrderType);
        Assert.NotEqual(Guid.Empty, order.ReferenceNumber);
    }

    [Fact]
    public void CreateDelivery_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        Product product = CreateProduct();
        List<Product> products = [product];

        // Act
        Order order = Order.CreateDelivery(DefaultId, DefaultCustomerId, DefaultPaymentId, products);

        // Assert
        Assert.Equal(DefaultId, order.Id);
        Assert.Equal(DefaultCustomerId, order.CustomerId);
        Assert.Equal(DefaultPaymentId, order.PaymentId);
        Assert.Equal(products, order.ProductId);
        Assert.Equal(OrderType.Delivery, order.OrderType);
        Assert.NotEqual(Guid.Empty, order.ReferenceNumber);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NoProductsAssigned()
    {
        // Arrange
        List<Product> products = [];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => Order.CreateDineIn(DefaultId, DefaultCustomerId, DefaultPaymentId, products));
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
        DomainException exception = Assert.Throws<DomainException>(() => Order.CreateDineIn(DefaultId, DefaultCustomerId, DefaultPaymentId, products));
        Assert.Equal(OrderErrors.TooManyProducts().Message, exception.Message);
    }

    private static Product CreateProduct()
    {
        return Product.Create(
            new ProductId(Guid.NewGuid()),
            new ProductId(Guid.NewGuid()),
            "Delicious Ramen",
            "Tasty Ramen Description that is long enough to meet the minimum length requirement.",
            25.0m,
            "http://example.com/image.jpg",
            [],
            []);
    }
}
