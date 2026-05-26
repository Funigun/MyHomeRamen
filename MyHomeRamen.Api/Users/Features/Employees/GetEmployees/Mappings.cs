using MyHomeRamen.Common.Contracts.Users.Employees.Responses;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Api.Users.Features.Employees.GetEmployees;

public static class Mappings
{
    public static EmployeeDto ToResponse(this KeycloakUserDto user)
        => new(user.Username, user.FirstName, user.LastName, user.Email);
}
