using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Roles.Responses;
using MyHomeRamen.Features.Common.Endpoints;

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
        [FromServices] IQueryHandler<GetAvailableRolesQuery, GetAvailableRolesResponse> handler,
        CancellationToken cancellationToken)
    {
        GetAvailableRolesQuery query = new();
        GetAvailableRolesResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
