using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Identity.Features.Users.GetAddresses;

public sealed class GetAddressesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetAddressesResponse>("api/account/me/addresses", HandleAsync)
                       .WithName("GetAddressesEndpoint")
                       .WithTags("account")
                       .WithDescription("Returns all addresses for the authenticated user.")
                       .RequireAuthorization(AuthorizationPolicies.AnyAuthenticatedPolicy);
    }

    private static async Task<Results<Ok<GetAddressesResponse>, NotFound>> HandleAsync(
        [FromServices] IQueryHandler<GetAddressesQuery, GetAddressesResponse> handler,
        CancellationToken cancellationToken)
    {
        GetAddressesQuery query = new();
        GetAddressesResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}

