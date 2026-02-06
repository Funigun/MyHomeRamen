using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Orders.Products;
using MyHomeRamen.Domain.Payments.Orders;

namespace MyHomeRamen.Domain.Orders.Payments;

public sealed class Payment : AuditableEntity, IEntity<PaymentId>
{
    private readonly List<Product> _productIds = [];

    public PaymentId Id { get; }

    public Guid ReferenceNumber { get; private set; }

    public decimal Amount { get; private set; }

    public Order Order { get; private set; }

    public IReadOnlyList<Product> Products => _productIds.ToList();

    private Payment()
    {
    }

    private Payment(PaymentId id, Order order, List<Product> products)
    {
        Id = id;
        Order = order;
        _productIds.AddRange(products);
    }

    public static Payment Create(PaymentId id, decimal amount, Order order, List<Product> products)
    {
        Payment payment = new(id, order, products)
        {
            ReferenceNumber = Guid.CreateVersion7(),
            Amount = amount
        };

        return payment;
    }
}
