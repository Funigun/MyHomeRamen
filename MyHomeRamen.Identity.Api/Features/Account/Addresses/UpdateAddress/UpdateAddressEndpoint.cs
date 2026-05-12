using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.UpdateAddress.Models;
using MyHomeRamen.Identity.Api.Presentation;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.UpdateAddress;

public sealed class UpdateAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedPutWithResponse<UpdateAddressRequest, UpdateAddressResponse>(
                "api/account/me/addresses/{id}", HandleAsync)
            .WithName("UpdateAddressEndpoint")
            .WithTags("account")
            .WithDescription("Updates an existing address of the authenticated user.")
            .RequireAuthorization(DependencyInjection.AnyAuthenticatedPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UpdateAddressRequestId id,
        [FromBody] UpdateAddressRequest request,
        [FromServices] IRequestHandler<UpdateAddressRequest, UpdateAddressResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateAddressResponse response = await handler.Handle(request with { Id = id.Id }, cancellationToken);

        return Results.Ok(response);
    }
}
