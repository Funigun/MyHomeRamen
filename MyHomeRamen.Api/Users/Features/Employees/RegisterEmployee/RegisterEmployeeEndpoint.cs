using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Users.Features.Employees.RegisterEmployee.Models;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Api.Users.Features.Employees.RegisterEmployee;

public sealed class RegisterEmployeeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<RegisterEmployeeRequest, Created>("api/admin/employee-sign-up", Handler)
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)
                       .WithName("CreateEmployeeEndpoint")
                       .WithTags("admin")
                       .WithDescription("Creates an employee account in Keycloak. Requires admin role.");
    }

    private static async Task<Results<Created, BadRequest>> Handler(
        RegisterEmployeeRequest request,
        [FromServices] IKeycloakAdminService keycloakAdminService,
        [FromServices] IUsersDbContext usersDbContext,
        CancellationToken cancellationToken)
    {
        KeycloakUserDto keycloakUser = request.ToUserDto();

        string keycloakUserId = await keycloakAdminService.CreateUserAsync(keycloakUser, RoleConstants.Employee, cancellationToken);

        User user = User.Create(
            keycloakUserId,
            request.Username,
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            RoleConstants.Employee
            );

        usersDbContext.Users.Add(user);
        await usersDbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Created();
    }
}
