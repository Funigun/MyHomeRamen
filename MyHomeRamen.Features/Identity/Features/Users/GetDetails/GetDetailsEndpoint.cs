using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.GetDetails;

public sealed record GetDetailsResponse(string Username, string FirstName, string LastName, string Email, string PhoneNumber);

public sealed class GetDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetDetailsResponse>("api/account/me", HandleAsync)
                       .WithName("GetDetailsEndpoint")
                       .WithTags("account")
                       .WithDescription("Returns the authenticated user's profile details.")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<Results<Ok<GetDetailsResponse>, NotFound>> HandleAsync(
        [FromServices] IRequestHandler<GetDetailsQuery, GetDetailsResponse> handler,
        CancellationToken cancellationToken)
    {
        GetDetailsQuery query = new();
        GetDetailsResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
