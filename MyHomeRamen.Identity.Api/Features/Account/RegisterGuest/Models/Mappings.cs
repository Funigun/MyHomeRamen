using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Identity.Api.Features.Account.RegisterGuest.Models;

public static class Mappings
{
    internal static RegisterGuestResponse ToRegisterGuestResponse(this User user)
    {
        return new RegisterGuestResponse(user.GuestId!.Value);
    }
}
