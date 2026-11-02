using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Payments.Orders;

public sealed class Order : AuditableEntity, IEntity<OrderId>
{
    public OrderId Id { get; private set; }

    public Guid CompanyId { get; private set; }

    public OrderId OriginalId { get; private set; }

    public decimal Amount { get; private set; }

    private Order()
    {
    }

    private Order(OrderId id, Guid companyId, OrderId originalId)
    {
        Id = id;
        CompanyId = companyId;
        OriginalId = originalId;
    }

    public static Order Create(OrderId id, Guid companyId, OrderId originalId, decimal amount)
    {
        Order order = new(id, companyId, originalId)
        {
            Amount = amount
        };

        OrderValidator.Validate(order);

        return order;
    }
}
