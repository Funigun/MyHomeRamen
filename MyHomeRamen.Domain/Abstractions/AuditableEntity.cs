namespace MyHomeRamen.Domain.Abstractions;

public abstract class AuditableEntity
{
    public Guid RestaurantId { get; set; }

    public string CreatedBy { get; set; } = default!;

    public DateTimeOffset CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTimeOffset? ModifiedOn { get; set; }

    public void SetRestaurantId(Guid restaurantId)
    {
        RestaurantId = restaurantId;
    }
}
