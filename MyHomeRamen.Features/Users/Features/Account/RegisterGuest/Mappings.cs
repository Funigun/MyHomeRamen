using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Features.Users.Features.Account.RegisterGuest;

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

