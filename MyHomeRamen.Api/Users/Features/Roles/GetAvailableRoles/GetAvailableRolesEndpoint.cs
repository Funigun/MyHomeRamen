using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Roles.Responses;

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
        [FromServices] IRequestHandler<GetAvailableRolesQuery, GetAvailableRolesResponse> handler,
        CancellationToken cancellationToken)
    {
        GetAvailableRolesResponse response = await handler.Handle(new GetAvailableRolesQuery(), cancellationToken);

        return TypedResults.Ok(response);
    }
}
