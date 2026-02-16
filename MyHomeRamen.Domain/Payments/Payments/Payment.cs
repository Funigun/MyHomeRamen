using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments.Payments;

public sealed class Payment : AuditableEntity, IEntity<PaymentId>
{
    public PaymentId Id { get; private set; }

    public Guid ReferenceId { get; private set; }

    public string Name { get; private set; }

    public string ImageUrl { get; private set; }

    private Payment()
    {
    }

    private Payment(PaymentId id, Guid referenceId)
    {
        Id = id;
        ReferenceId = referenceId;
    }

    public static Payment Create(PaymentId id, Guid referenceId, string name, string imageUrl)
    {
        Payment payment = new(id, referenceId)
        {
            Name = name,
            ImageUrl = imageUrl
        };

        PaymentValidator.Validate(payment);

        return payment;
    }
}
