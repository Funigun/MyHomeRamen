using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Users.Features.Account.CreateAddress.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Users.Features.Account.CreateAddress;

public sealed class AddAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPost<AddAddressRequest, AddAddressResponse>("api/account/me/addresses", HandleAsync)
                       .WithName("AddAddressEndpoint")
                       .WithTags("account")
                       .WithDescription("Adds a new address to the authenticated user's profile.")
                       .RequireAuthorization(AuthorizationDependencyInjection.AnyAuthenticatedPolicy);
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
