using MyHomeRamen.Common.Contracts.Users.Employees.Responses;
using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Features.Identity.Features.Users.GetEmployees;

public static class Mappings
{
    public static EmployeeDto ToResponse(this KeycloakUserDto user)
        => new(user.Username, user.FirstName, user.LastName, user.Email);
}

