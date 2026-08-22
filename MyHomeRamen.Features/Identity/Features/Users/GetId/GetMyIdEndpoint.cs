using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Identity.Features.Users.GetId;

public sealed record GetMyIdResponse(Guid Id);

public sealed class GetMyIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetMyIdResponse>("api/account/me/id", HandleAsync)
                       .WithName("GetMyIdEndpoint")
                       .WithTags("account")
                       .WithDescription("Returns the authenticated user's internal ID.");
    }

    private static async Task<Results<Ok<GetMyIdResponse>, NotFound>> HandleAsync(
        [FromServices] IQueryHandler<GetMyIdQuery, GetMyIdResponse> handler,
        CancellationToken cancellationToken)
    {
        GetMyIdQuery query = new();
        GetMyIdResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
