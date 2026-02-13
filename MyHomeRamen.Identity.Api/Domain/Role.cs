using Microsoft.AspNetCore.Identity;

namespace MyHomeRamen.Identity.Api.Domain;

public class Role : IdentityRole<Guid>
{
    public Guid RestaurantId { get; set; }
}
