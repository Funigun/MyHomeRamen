using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Payments.PaymentProviders;

namespace MyHomeRamen.Domain.Payments.PaymentGroups;

public sealed class PaymentGroup : AuditableEntity, IEntity<PaymentGroupId>
{
    private readonly List<PaymentProvider> _paymentProviders = [];

    public PaymentGroupId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public string Name { get; private set; }

    public string ImageUrl { get; private set; }

    public IReadOnlyList<PaymentProvider> PaymentProviders => _paymentProviders.ToList();

    private PaymentGroup()
    {
    }

    private PaymentGroup(PaymentGroupId id, Guid restaurantId, IEnumerable<PaymentProvider> paymentProviders)
    {
        Id = id;
        RestaurantId = restaurantId;
        _paymentProviders = paymentProviders.ToList();
    }

    public static PaymentGroup Create(PaymentGroupId id, Guid restaurantId, string name, string imageUrl, IEnumerable<PaymentProvider> paymentProviders)
    {
        PaymentGroup paymentGroup = new(id, restaurantId, paymentProviders)
        {
            Name = name,
            ImageUrl = imageUrl
        };

        PaymentGroupValidator.Validate(paymentGroup);
        return paymentGroup;
    }
}
