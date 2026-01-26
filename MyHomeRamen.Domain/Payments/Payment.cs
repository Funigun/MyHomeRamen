using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments;

public sealed class Payment : AuditableEntity, IEntity<PaymentId>
{
    public PaymentId Id { get; private set; }

    public string Name { get; private set; }

    public string ImageUrl { get; private set; }
}
