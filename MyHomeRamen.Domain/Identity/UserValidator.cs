using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Domain.Identity;

internal static class UserValidator
{
    internal static void ValidateUser(User user)
    {
        bool hasKeycloak = !string.IsNullOrWhiteSpace(user.KeycloakUserId);
        bool hasGuest = user.GuestId.HasValue;

        if ((hasKeycloak && hasGuest) || (!hasKeycloak && !hasGuest))
        {
            throw UserErrors.InvalidIdentity();
        }
    }
}
