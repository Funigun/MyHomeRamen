using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.Register.Models;
using MyHomeRamen.Identity.Api.Features.Admin.RegisterEmployee.Models;
using MyHomeRamen.Identity.Api.Presentation;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Identity.Api.Features.Admin.RegisterEmployee;

public sealed class RegisterEmployeeEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Admin";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<RegisterEmployeeRequest, Created>("/employee-sign-up", Handler)
                       .RequireAuthorization(DependencyInjection.RestaurantManagerPolicy)
                       .WithName("CreateEmployeeEndpoint")
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
            RoleConstants.Customer
            );

        usersDbContext.Users.Add(user);
        await usersDbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Created();
    }
}
