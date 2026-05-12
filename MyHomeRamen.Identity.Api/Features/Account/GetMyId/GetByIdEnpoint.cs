using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Identity.Api.Features.Account.GetMyId.Models;
using MyHomeRamen.Identity.Api.Presentation;

namespace MyHomeRamen.Identity.Api.Features.Account.GetMyId;

public sealed class GetByIdEnpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetMyIdResponse>("api/account/me/id", HandleAsync)
                       .WithName("GetMyIdEndpoint")
                       .WithTags("account")
                       .WithDescription("Returns the authenticated user's internal ID.")
                       .RequireAuthorization(DependencyInjection.AnyAuthenticatedPolicy);
    }

    private static async Task<Results<Ok<GetMyIdResponse>, NotFound>> HandleAsync(
        [FromServices] IRequestHandler<GetMyIdRequest, GetMyIdResponse> handler,
        CancellationToken cancellationToken)
    {
        GetMyIdResponse response = await handler.Handle(new GetMyIdRequest(), cancellationToken);

        return TypedResults.Ok(response);
    }
}
