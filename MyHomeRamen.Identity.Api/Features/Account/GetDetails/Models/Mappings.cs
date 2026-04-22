using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Identity.Api.Features.Account.GetDetails.Models;

internal static class Mappings
{
    extension(User user)
    {
        internal GetDetailsResponse ToGetDetailsResponse()
        {
            return new GetDetailsResponse(
                user.UserName!,
                user.FirstName,
                user.LastName,
                user.Email!,
                user.PhoneNumber!);
        }
    }
}
