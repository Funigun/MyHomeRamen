using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Users.Features.Account.GetAddresses.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Users.Features.Account.GetAddresses;

public sealed class GetAddressesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetAddressesResponse>("api/account/me/addresses", HandleAsync)
                       .WithName("GetAddressesEndpoint")
                       .WithTags("account")
                       .WithDescription("Returns all addresses for the authenticated user.")
                       .RequireAuthorization(AuthorizationDependencyInjection.AnyAuthenticatedPolicy);
    }

    private static async Task<Results<Ok<GetAddressesResponse>, NotFound>> HandleAsync(
        [FromServices] IRequestHandler<GetAddressesRequest, GetAddressesResponse> handler,
        CancellationToken cancellationToken)
    {
        GetAddressesResponse response = await handler.Handle(new GetAddressesRequest(), cancellationToken);

        return TypedResults.Ok(response);
    }
}
