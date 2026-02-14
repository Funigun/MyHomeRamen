using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Identity.Api.Features.Account.Register.Models;

internal static class Mappings
{
    extension(RegisterRequest request)
    {
        internal User ToUser(Guid restaurantId)
        {
            return User.Create
            (
                restaurantId,
                request.UserName,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber
            );
        }
    }
}
