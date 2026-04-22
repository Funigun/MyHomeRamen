using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.GetAddresses.Models;
using MyHomeRamen.Identity.Api.Presentation;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.GetAddresses;

public sealed class GetAddressesEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Account";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetAddressesResponse>("/me/addresses", HandleAsync)
                       .WithName("GetAddressesEndpoint")
                       .WithDescription("Returns all addresses for the authenticated user.")
                       .RequireAuthorization(DependencyInjection.AnyAuthenticatedPolicy);
    }

    private static async Task<Results<Ok<GetAddressesResponse>, NotFound>> HandleAsync(
        [FromServices] IRequestHandler<GetAddressesRequest, GetAddressesResponse> handler,
        CancellationToken cancellationToken)
    {
        GetAddressesResponse response = await handler.Handle(new GetAddressesRequest(), cancellationToken);

        return TypedResults.Ok(response);
    }
}
