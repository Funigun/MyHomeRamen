using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.GetId;

public sealed record GetMyIdResponse(Guid Id);

public sealed class GetMyIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetMyIdResponse>("api/account/me/id", HandleAsync)
                       .WithName("GetMyIdEndpoint")
                       .WithTags("account")
                       .WithDescription("Returns the authenticated user's internal ID.")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<Results<Ok<GetMyIdResponse>, NotFound>> HandleAsync(
        [FromServices] IRequestHandler<GetMyIdQuery, GetMyIdResponse> handler,
        CancellationToken cancellationToken)
    {
        GetMyIdQuery query = new();
        GetMyIdResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
