using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Identity.Api.Presentation;
using MyHomeRamen.Infrastructure.Keycloak;

namespace MyHomeRamen.Identity.Api.Features.Admin.CreateEmployee;

public sealed class CreateEmployeeEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Admin";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateEmployeeRequest, Created>("/employees", Handler)
               //.RequireAuthorization(DependencyInjection.AdminPolicy)
               .AllowAnonymous()
               .WithName("CreateEmployeeEndpoint")
               .WithDescription("Creates an employee account in Keycloak. Requires admin role.");
    }

    private static async Task<Results<Created, BadRequest>> Handler(
        CreateEmployeeRequest request,
        [FromServices] IKeycloakAdminService keycloakAdminService,
        CancellationToken cancellationToken)
    {
        KeycloakUserRepresentation keycloakUser = new()
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Enabled = true,
            Credentials =
            [
                new KeycloakCredentialRepresentation
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
