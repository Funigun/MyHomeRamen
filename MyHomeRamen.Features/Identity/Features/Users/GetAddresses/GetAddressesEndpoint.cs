using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.GetAddresses;

public sealed record GetAddressesResponse(IEnumerable<AddressDto> Addresses);

public sealed record AddressDto(Guid Id, string Street, string Building, string Apartment, string City, string ZipCode, bool IsDefault);

public sealed class GetAddressesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetAddressesResponse>("api/account/me/addresses", HandleAsync)
                       .WithName("GetAddressesEndpoint")
                       .WithTags("account")
                       .WithDescription("Returns all addresses for the authenticated user.")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<Results<Ok<GetAddressesResponse>, NotFound>> HandleAsync(
        [FromServices] IRequestHandler<GetAddressesQuery, GetAddressesResponse> handler,
        CancellationToken cancellationToken)
    {
        GetAddressesQuery query = new();
        GetAddressesResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
