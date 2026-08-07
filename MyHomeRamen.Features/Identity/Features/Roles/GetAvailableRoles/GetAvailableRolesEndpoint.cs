using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Identity.Features.Roles.GetAvailableRoles;

public sealed record GetAvailableRolesResponse(IEnumerable<RoleDto> Roles);

public sealed record RoleDto(string Id, string Name, string Description);

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
        [FromServices] IQueryHandler<GetAvailableRolesQuery, GetAvailableRolesResponse> handler,
        CancellationToken cancellationToken)
    {
        GetAvailableRolesQuery query = new();
        GetAvailableRolesResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}

