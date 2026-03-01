using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Identity.Api.Features.Admin.RegisterEmployee.Models;

public static class Mappings
{
    public static KeycloakUserDto ToUserDto(this RegisterEmployeeRequest request)
    {
        return new KeycloakUserDto
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Enabled = true,
            Credentials =
            [
                new KeycloakCredentialDto
                {
                    Type = "password",
                    Value = request.TemporaryPassword,
                    Temporary = false,
                }
            ]
        };
    }
}
