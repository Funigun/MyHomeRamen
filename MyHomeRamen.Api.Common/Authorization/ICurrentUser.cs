using System.Security.Claims;

namespace MyHomeRamen.Api.Common.Authorization;

public interface ICurrentUser
{
    string Id { get; init; }

    Guid UserId { get; init; }

    Guid RestaurantId { get; init; }

    IEnumerable<Claim> Claims { get; init; }
}
