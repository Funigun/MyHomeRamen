using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Identity.Api.Features.Admin.RegisterEmployee.Models;

internal static class Mappings
{
    extension(RegisterEmployeeRequest request)
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
