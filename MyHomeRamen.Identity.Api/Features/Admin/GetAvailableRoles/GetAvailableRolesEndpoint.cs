using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Identity.Api.Features.Admin.GetAvailableRoles.Models;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Identity.Api.Features.Admin.GetAvailableRoles;

public sealed class GetAvailableRolesEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Admin";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetAvailableRolesResponse>("/role", Handler)
                       .WithName("GetAvailableRolesEndpoint")
                       .WithDescription("Gets the user available roles.")
                       .AllowAnonymous();
    }

    private static async Task<Results<Ok<GetAvailableRolesResponse>, NotFound>> Handler(
        [FromServices] IKeycloakAdminService keycloakAdminService,
        CancellationToken cancellationToken)
    {
        IEnumerable<KeycloakRoleDto> roles = await keycloakAdminService.GetAvailableRoles(cancellationToken);

        return TypedResults.Ok(roles.ToResponse());
    }
}
