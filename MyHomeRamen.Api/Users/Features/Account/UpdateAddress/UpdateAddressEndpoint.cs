using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Users.Features.Account.UpdateAddress.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Users.Features.Account.UpdateAddress;

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
            .RequireAuthorization(AuthorizationDependencyInjection.AnyAuthenticatedPolicy);
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
