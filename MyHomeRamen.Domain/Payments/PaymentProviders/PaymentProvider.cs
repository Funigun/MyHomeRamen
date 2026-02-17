using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Payments.Payments;

namespace MyHomeRamen.Domain.Payments.PaymentProviders;

public sealed class PaymentProvider : AuditableEntity, IEntity<PaymentProviderId>
{
    private readonly List<Payment> _paymentMethods = [];

    public PaymentProviderId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public string Name { get; private set; }

    public string ImageUrl { get; private set; }

    public ICollection<Payment> Payments => _paymentMethods.ToList();

    private PaymentProvider()
    {
    }

    private PaymentProvider(PaymentProviderId id, Guid restaurantId)
    {
        Id = id;
        RestaurantId = restaurantId;
    }

    public static PaymentProvider Create(PaymentProviderId id, Guid restaurantId, string name, string imageUrl)
    {
        PaymentProvider paymentProvider = new(id, restaurantId)
        {
            Name = name,
            ImageUrl = imageUrl
        };

        PaymentProviderValidator.Validate(paymentProvider);
        return paymentProvider;
    }
}
