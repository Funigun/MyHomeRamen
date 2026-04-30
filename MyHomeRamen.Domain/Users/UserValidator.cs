using MyHomeRamen.Domain.Common.User;

namespace MyHomeRamen.Domain.Users;

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
