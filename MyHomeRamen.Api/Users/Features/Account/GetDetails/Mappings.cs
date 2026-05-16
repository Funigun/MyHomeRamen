using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Api.Users.Features.Account.GetDetails;

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
