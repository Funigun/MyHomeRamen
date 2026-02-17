using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Domain.Payments.Payments;

public sealed class Payment : AuditableEntity, IEntity<PaymentId>
{
    private readonly List<User> _users = new();

    public PaymentId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public Guid ReferenceId { get; private set; }

    public string Name { get; private set; }

    public string ImageUrl { get; private set; }

    public ICollection<User> Users => _users.ToList();

    private Payment()
    {
    }

    private Payment(PaymentId id, Guid restaurantId, Guid referenceId)
    {
        Id = id;
        RestaurantId = restaurantId;
        ReferenceId = referenceId;
    }

    public static Payment Create(PaymentId id, Guid restaurantId, Guid referenceId, string name, string imageUrl)
    {
        Payment payment = new(id, restaurantId, referenceId)
        {
            Name = name,
            ImageUrl = imageUrl
        };

        PaymentValidator.Validate(payment);

        return payment;
    }
}
