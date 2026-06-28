using MyHomeRamen.Common.Contracts.Users.Employees.Requests;
using MyHomeRamen.Features.Users.Services.Dto;

namespace MyHomeRamen.Features.Users.Features.Employees.RegisterEmployee;

public static class Mappings
{
    public static KeycloakUserDto ToUserDto(this RegisterEmployeeRequest request)
        => new()
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Credentials = [new KeycloakCredentialDto { Value = request.TemporaryPassword, Temporary = true }]
        };
}

