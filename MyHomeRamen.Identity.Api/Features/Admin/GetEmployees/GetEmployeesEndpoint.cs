using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Identity.Api.Features.Admin.GetEmployees.Models;
using MyHomeRamen.Identity.Api.Presentation;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Identity.Api.Features.Admin.GetEmployees;

public sealed class GetEmployeesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetEmployeesResponse>("api/admin/employee", Handler)
                       .RequireAuthorization(DependencyInjection.RestaurantManagerPolicy)
                       .WithName("GetEmployeesEndpoint")
                       .WithTags("admin")
                       .WithDescription("Handles GetEmployees operations.");
    }

    private static async Task<Results<Ok<GetEmployeesResponse>, NotFound>> Handler([FromServices] IKeycloakAdminService adminService, [FromServices] ICurrentUser current, CancellationToken cancellationToken)
    {
        IEnumerable<KeycloakUserDto> users = await adminService.GetEmployees(cancellationToken);

        GetEmployeesResponse response = new(users.Select(s => s.ToResponse()));

        return TypedResults.Ok(response);
    }
}
