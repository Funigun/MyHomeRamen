using Microsoft.AspNetCore.Identity;

namespace MyHomeRamen.Domain.Users;

public class Role : IdentityRole<Guid>
{
    public Guid RestaurantId { get; set; }

    public void SetRestaurantId(Guid restaurantId)
    {
        RestaurantId = restaurantId;
    }
}
