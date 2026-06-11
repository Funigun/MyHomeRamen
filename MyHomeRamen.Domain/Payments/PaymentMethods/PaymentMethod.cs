using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Payments.PaymentChannels;

namespace MyHomeRamen.Domain.Payments.PaymentMethods;

public sealed class PaymentMethod : AuditableEntity, IEntity<PaymentMethodId>
{
    private readonly List<PaymentChannel> _paymentChannels = [];

    public PaymentMethodId Id { get; private set; }

    public string Name { get; private set; }

    public string ImageUrl { get; private set; }

    public bool IsActive { get; private set; }

    public int DisplayOrder { get; private set; }

    public IReadOnlyList<PaymentChannel> PaymentChannels => _paymentChannels.AsReadOnly();

    private PaymentMethod() { }

    public static PaymentMethod Create(PaymentMethodId id, string name, string imageUrl, bool isActive, int displayOrder)
    {
        PaymentMethod paymentMethod = new()
        {
            Id = id,
            Name = name,
            ImageUrl = imageUrl,
            IsActive = isActive,
            DisplayOrder = displayOrder
        };

        PaymentMethodValidator.Validate(paymentMethod);

        return paymentMethod;
    }
}
