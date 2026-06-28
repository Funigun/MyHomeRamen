using MyHomeRamen.Common.Contracts.Users.Employees.Responses;
using MyHomeRamen.Features.Users.Services.Dto;

namespace MyHomeRamen.Features.Users.Features.Employees.GetEmployees;

public static class Mappings
{
    public static EmployeeDto ToResponse(this KeycloakUserDto user)
        => new(user.Username, user.FirstName, user.LastName, user.Email);
}

