using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders;

public sealed class Order : AuditableEntity, IEntity<OrderId>, IEventProducer
{
    private readonly List<Product> _productIds = [];
    private readonly List<IDomainEvent> _events = [];

    public OrderId Id { get; private set; }

    public Guid ReferenceNumber { get; private set; }

    public CustomerId CustomerId { get; private set; }

    public PaymentId PaymentId { get; private set; }

    public OrderType OrderType { get; private set; }

    public IReadOnlyList<Product> ProductId => _productIds.ToList();

    public IReadOnlyList<IDomainEvent> Events => _events.ToList();

    private Order()
    {
    }

    private Order(OrderId id, CustomerId customerId, PaymentId paymentId, IEnumerable<Product> productIds)
    {
        Id = id;
        ReferenceNumber = Guid.CreateVersion7();
        CustomerId = customerId;
        PaymentId = paymentId;
        _productIds.AddRange(productIds);
    }

    public static Order CreateDineIn(OrderId id, CustomerId customerId, PaymentId paymentId, IEnumerable<Product> productIds)
    {
        return new(id, customerId, paymentId, productIds)
        {
            OrderType = OrderType.DineIn
        };
    }

    public static Order CreateTakeOut(OrderId id, CustomerId customerId, PaymentId paymentId, IEnumerable<Product> productIds)
    {
        return new(id, customerId, paymentId, productIds)
        {
            OrderType = OrderType.TakeOut
        };
    }

    public static Order CreateDelivery(OrderId id, CustomerId customerId, PaymentId paymentId, IEnumerable<Product> productIds)
    {
        return new(id, customerId, paymentId, productIds)
        {
            OrderType = OrderType.Delivery
        };
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
