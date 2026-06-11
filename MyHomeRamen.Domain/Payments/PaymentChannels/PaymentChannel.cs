using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Payments.PaymentGateways;

namespace MyHomeRamen.Domain.Payments.PaymentChannels;

public sealed class PaymentChannel : AuditableEntity, IEntity<PaymentChannelId>
{
    public PaymentChannelId Id { get; private set; }

    public string Name { get; private set; }

    public string ImageUrl { get; private set; }

    public bool IsActive { get; private set; }

    public int DisplayOrder { get; private set; }

    public PaymentGateway PaymentGateway { get; private set; }

    private PaymentChannel() { }

    public static PaymentChannel Create(PaymentChannelId id, string name, string imageUrl, bool isActive, int displayOrder, PaymentGateway paymentGateway)
    {
        PaymentChannel paymentChannel = new()
        {
            Id = id,
            Name = name,
            ImageUrl = imageUrl,
            IsActive = isActive,
            DisplayOrder = displayOrder,
            PaymentGateway = paymentGateway
        };

        PaymentChannelValidator.Validate(paymentChannel);

        return paymentChannel;
    }
}
