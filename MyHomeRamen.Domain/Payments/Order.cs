using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments;

public sealed class Order : AuditableEntity, IEntity<OrderId>
{
    public OrderId Id { get; private set; }

    public OrderId OriginalId { get; private set; }

    public decimal Amount { get; private set; }

    private Order()
    {
    }

    private Order(OrderId id, OrderId originalId)
    {
        Id = id;
        OriginalId = originalId;
    }

    public static Order Create(OrderId id, OrderId originalId, decimal amount)
    {
        return new Order(id, originalId)
        {
            Amount = amount
        };
    }
}
