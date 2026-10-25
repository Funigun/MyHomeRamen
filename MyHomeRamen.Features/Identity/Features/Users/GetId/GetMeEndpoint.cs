using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.GetId;

public sealed class GetMeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetMeResponse>("api/identity/users/me", HandleAsync)
                       .WithName("GetMeEndpoint")
                       .WithTags("identity", "users")
                       .WithDescription("Returns the current user's identity and available panel actions.")
                       .AllowAnonymous();
    }

    private static async Task<Ok<GetMeResponse>> HandleAsync(
        [FromServices] IRequestHandler<GetMeQuery, GetMeResponse> handler,
        CancellationToken cancellationToken)
    {
        GetMeQuery query = new();
        GetMeResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
