using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Orders.Payments;
using MyHomeRamen.Domain.Orders.Products;
using MyHomeRamen.Domain.Orders.Users;

namespace MyHomeRamen.Domain.Orders.Orders;

public sealed class Order : AuditableEntity, IEntity<OrderId>, IEventProducer
{
    private readonly List<Product> _productIds = [];
    private readonly List<Payment> _payments = [];
    private readonly List<IDomainEvent> _events = [];

    public OrderId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public Guid ReferenceNumber { get; private set; }

    public OrderType Type { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal TotalOriginalAmount { get; private set; }

    public decimal TotalCalculatedAmount { get; private set; }

    public User User { get; private set; }

    public OrderAddress DeliveryAddress { get; private set; }

    public IReadOnlyList<Product> Products => _productIds.ToList();

    public IReadOnlyList<Payment> Payments => _payments.ToList();

    public IReadOnlyList<IDomainEvent> Events => _events.ToList();

    private Order()
    {
    }

    private Order(OrderId id, Guid restaurantId, IEnumerable<Product> productIds)
    {
        Id = id;
        RestaurantId = restaurantId;
        ReferenceNumber = Guid.CreateVersion7();
        TotalOriginalAmount = productIds.Sum(p => p.OriginalPrice);
        Status = OrderStatus.Created;
        _productIds.AddRange(productIds);
    }

    public static Order CreateDineIn(OrderId id, Guid restaurantId, IEnumerable<Product> productIds)
    {
        Order order = new(id, restaurantId, productIds)
        {
            Type = OrderType.DineIn
        };

        OrderValidator.Validate(order);

        return order;
    }

    public static Order CreateTakeOut(OrderId id, Guid restaurantId, IEnumerable<Product> productIds)
    {
        Order order = new(id, restaurantId, productIds)
        {
            Type = OrderType.TakeOut
        };

        OrderValidator.Validate(order);

        return order;
    }

    public static Order CreateDelivery(OrderId id, Guid restaurantId, IEnumerable<Product> productIds)
    {
        Order order = new(id, restaurantId, productIds)
        {
            Type = OrderType.Delivery
        };

        OrderValidator.Validate(order);

        return order;
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _events.Clear();
    }
}
