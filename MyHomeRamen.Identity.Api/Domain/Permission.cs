namespace MyHomeRamen.Identity.Api.Domain;

public class Permission
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid RestaurantId { get; set; }
}
