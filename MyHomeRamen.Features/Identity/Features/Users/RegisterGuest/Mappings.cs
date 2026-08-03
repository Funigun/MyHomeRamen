using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Features.Identity.Features.Users.RegisterGuest;

internal static class Mappings
{
    extension(User user)
    {
        internal RegisterGuestResponse ToRegisterGuestResponse()
        {
            return new RegisterGuestResponse(user.GuestId!.Value);
        }
    }
}

