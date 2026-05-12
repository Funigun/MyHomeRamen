using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.AddAddress.Models;
using MyHomeRamen.Identity.Api.Presentation;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.AddAddress;

public sealed class AddAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPost<AddAddressRequest, AddAddressResponse>("api/account/me/addresses", HandleAsync)
                       .WithName("AddAddressEndpoint")
                       .WithTags("account")
                       .WithDescription("Adds a new address to the authenticated user's profile.")
                       .RequireAuthorization(DependencyInjection.AnyAuthenticatedPolicy);
    }

    private static async Task<Results<Created<AddAddressResponse>, BadRequest>> HandleAsync(
        [FromBody] AddAddressRequest request,
        [FromServices] IRequestHandler<AddAddressRequest, Guid> handler,
        CancellationToken cancellationToken)
    {
        Guid addressId = await handler.Handle(request, cancellationToken);

        AddAddressResponse response = new(addressId);

        return TypedResults.Created($"/api/account/me/addresses/{addressId}", response);
    }
}
