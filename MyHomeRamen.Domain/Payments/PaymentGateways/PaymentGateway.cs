using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Payments.PaymentGateways;

public sealed class PaymentGateway : AuditableEntity, IEntity<PaymentGatewayId>
{
    public PaymentGatewayId Id { get; private set; }

    public string Name { get; private set; } = null!;

    private PaymentGateway() { }

    public static PaymentGateway Create(PaymentGatewayId id, string name)
    {
        PaymentGateway paymentGateway = new()
        {
            Id = id,
            Name = name
        };

        PaymentGatewayValidator.Validate(paymentGateway);

        return paymentGateway;
    }
}
