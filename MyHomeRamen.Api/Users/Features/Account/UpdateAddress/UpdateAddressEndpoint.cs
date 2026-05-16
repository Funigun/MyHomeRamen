using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Api.Users.Features.Account.UpdateAddress;

public sealed class UpdateAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedPutWithResponse<UpdateAddressCommand, UpdateAddressResponse>(
                "api/account/me/addresses/{id}", HandleAsync)
            .WithName("UpdateAddressEndpoint")
            .WithTags("account")
            .WithDescription("Updates an existing address of the authenticated user.")
            .RequireAuthorization(AuthorizationDependencyInjection.AnyAuthenticatedPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateAddressRequest request,
        [FromServices] IRequestHandler<UpdateAddressCommand, UpdateAddressResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateAddressResponse response = await handler.Handle(new UpdateAddressCommand(id, request), cancellationToken);

        return Results.Ok(response);
    }
}
