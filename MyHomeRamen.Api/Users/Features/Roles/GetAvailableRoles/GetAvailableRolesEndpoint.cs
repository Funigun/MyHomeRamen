using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Users.Features.Roles.GetAvailableRoles.Models;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Api.Users.Features.Roles.GetAvailableRoles;

public sealed class GetAvailableRolesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetAvailableRolesResponse>("api/admin/role", Handler)
                       .WithName("GetAvailableRolesEndpoint")
                       .WithTags("admin")
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
