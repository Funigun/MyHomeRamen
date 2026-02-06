using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments.PaymentProviders;

public sealed class PaymentProvider : AuditableEntity, IEntity<PaymentProviderId>
{
    public PaymentProviderId Id { get; private set; }

    public string Name { get; private set; }

    public string ImageUrl { get; private set; }

    private PaymentProvider()
    {
    }

    private PaymentProvider(PaymentProviderId id)
    {
        Id = id;
    }

    public static PaymentProvider Create(PaymentProviderId id, string name, string imageUrl)
    {
        PaymentProvider paymentProvider = new(id)
        {
            Name = name,
            ImageUrl = imageUrl
        };

        PaymentProviderValidator.Validate(paymentProvider);
        return paymentProvider;
    }
}
