using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Users.Features.Account.GetDetails.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Users.Features.Account.GetDetails;

public sealed class GetDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetDetailsResponse>("api/account/me", HandleAsync)
                       .WithName("GetDetailsEndpoint")
                       .WithTags("account")
                       .WithDescription("Returns the authenticated user's profile details.")
                       .RequireAuthorization(AuthorizationDependencyInjection.AnyAuthenticatedPolicy);
    }

    private static async Task<Results<Ok<GetDetailsResponse>, NotFound>> HandleAsync(
        [FromServices] IRequestHandler<GetDetailsRequest, GetDetailsResponse> handler,
        CancellationToken cancellationToken)
    {
        GetDetailsResponse response = await handler.Handle(new GetDetailsRequest(), cancellationToken);

        return TypedResults.Ok(response);
    }
}
