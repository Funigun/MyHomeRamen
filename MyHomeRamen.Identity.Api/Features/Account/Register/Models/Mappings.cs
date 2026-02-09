using MyHomeRamen.Identity.Api.Domain;

namespace MyHomeRamen.Identity.Api.Features.Account.Register.Models;

internal static class Mappings
{
    extension(RegisterRequest request)
    {
        internal User ToUser()
        {
            return User.Create
            (
                userName: request.UserName,
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                phoneNumber: request.PhoneNumber
            );
        }
    }
}
