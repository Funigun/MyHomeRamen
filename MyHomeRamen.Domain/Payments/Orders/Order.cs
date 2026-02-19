using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments.Orders;

public sealed class Order : AuditableEntity, IEntity<OrderId>
{
    public OrderId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public OrderId OriginalId { get; private set; }

    public decimal Amount { get; private set; }

    private Order()
    {
    }

    private Order(OrderId id, Guid restaurantId, OrderId originalId)
    {
        Id = id;
        RestaurantId = restaurantId;
        OriginalId = originalId;
    }

    public static Order Create(OrderId id, Guid restaurantId, OrderId originalId, decimal amount)
    {
        Order order = new(id, restaurantId, originalId)
        {
            Amount = amount
        };

        OrderValidator.Validate(order);

        return order;
    }
}
