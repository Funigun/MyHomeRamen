using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Features.Identity.Features.Users.GetDetails;

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

