using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Identity.Api.Features.Admin.GetEmployees.Models;

internal static class Mappings
{
    internal static EmployeeDto ToResponse(this KeycloakUserDto user)
    {
        return new EmployeeDto
        (
            Guid.CreateVersion7(),
            user.Username!,
            user.FirstName,
            user.LastName,
            user.Email!,
            ""
        );
    }
}
