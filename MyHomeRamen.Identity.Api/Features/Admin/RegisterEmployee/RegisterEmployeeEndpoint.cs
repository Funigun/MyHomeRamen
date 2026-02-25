using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint;
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
                       //.RequireAuthorization(DependencyInjection.AdminPolicy)
                       .AllowAnonymous()
                       .WithName("CreateEmployeeEndpoint")
                       .WithDescription("Creates an employee account in Keycloak. Requires admin role.");
    }

    private static async Task<Results<Created, BadRequest>> Handler(
        RegisterEmployeeRequest request,
        [FromServices] IKeycloakAdminService keycloakAdminService,
        CancellationToken cancellationToken)
    {
        KeycloakUserDto keycloakUser = new()
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

        await keycloakAdminService.CreateUserAsync(keycloakUser, cancellationToken);

        return TypedResults.Created();
    }
}
