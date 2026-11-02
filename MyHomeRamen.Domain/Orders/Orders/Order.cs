using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Orders.Payments;
using MyHomeRamen.Domain.Orders.Products;
using MyHomeRamen.Domain.Orders.Users;

namespace MyHomeRamen.Domain.Orders.Orders;

public sealed class Order : Aggregate<OrderId>
{
    private readonly List<Product> _productIds = [];
    private readonly List<Payment> _payments = [];

    public Guid ReferenceNumber { get; private set; }

    public OrderType Type { get; private set; }

    public OrderStatus Status { get; private set; }

    public Guid RestaurantId { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public decimal TotalOriginalAmount { get; private set; }

    public decimal TotalCalculatedAmount { get; private set; }

    public UserId UserId { get; private set; } = default!;

    public OrderAddress DeliveryAddress { get; private set; } = default!;

    public IReadOnlyList<Product> Products => _productIds.ToList();

    public IReadOnlyList<Payment> Payments => _payments.ToList();

    private Order()
    {
    }

    private Order(OrderId id, UserId userId, Guid restaurantId, string currency, IEnumerable<Product> productIds)
    {
        Id = id;
        UserId = userId;
        RestaurantId = restaurantId;
        Currency = currency.ToUpperInvariant();
        ReferenceNumber = Guid.CreateVersion7();
        TotalOriginalAmount = productIds.Sum(p => p.FinalChargedLinePrice);
        TotalCalculatedAmount = TotalOriginalAmount;
        Status = OrderStatus.Created;
        _productIds.AddRange(productIds);
    }

    public static Order CreateDineIn(OrderId id, UserId userId, Guid restaurantId, string currency, IEnumerable<Product> productIds)
    {
        Order order = new(id, userId, restaurantId, currency, productIds)
        {
            Type = OrderType.DineIn
        };

        OrderValidator.Validate(order);

        return order;
    }

    public static Order CreateTakeOut(OrderId id, UserId userId, Guid restaurantId, string currency, IEnumerable<Product> productIds)
    {
        Order order = new(id, userId, restaurantId, currency, productIds)
        {
            Type = OrderType.TakeOut
        };

        OrderValidator.Validate(order);

        return order;
    }

    public static Order CreateDelivery(
        OrderId id,
        UserId userId,
        Guid restaurantId,
        string currency,
        OrderAddress deliveryAddress,
        IEnumerable<Product> productIds)
    {
        Order order = new(id, userId, restaurantId, currency, productIds)
        {
            Type = OrderType.Delivery,
            DeliveryAddress = deliveryAddress
        };

        OrderValidator.Validate(order);

        return order;
    }
}
