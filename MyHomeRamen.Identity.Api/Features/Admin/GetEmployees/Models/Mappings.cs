using MyHomeRamen.Identity.Api.Domain;

namespace MyHomeRamen.Identity.Api.Features.Admin.GetEmployees.Models;

internal static class Mappings
{
    internal static EmployeeDto ToResponse(this User user)
    {
        return new EmployeeDto
        (
            user.Id,
            user.UserName!,
            user.FirstName,
            user.LastName,
            user.Email!,
            user.PhoneNumber!
        );
    }
}
