using MyHomeRamen.Domain.Users;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Identity.Api.Features.Account.Register.Models;

internal static class Mappings
{
    extension(RegisterRequest request)
    {
        internal KeycloakUserDto ToKeycloakUserDto()
        {
            return new KeycloakUserDto
            {
                Username = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Enabled = true,
                Credentials =
                [
                    new KeycloakCredentialDto
                    {
                        Type = "password",
                        Value = request.Password,
                        Temporary = false,
                    }
                ]
            };
        }
    }

    extension(RegisterRequest request)
    {
        internal User ToUserDto(string keycloakUserId, string role)
        {
            return User.Create(
                keycloakUserId,
                request.UserName,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                role
            );
        }
    }
}
