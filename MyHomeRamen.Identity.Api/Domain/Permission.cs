namespace MyHomeRamen.Identity.Api.Domain;

public class Permission
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid RestaurantId { get; set; }
}
