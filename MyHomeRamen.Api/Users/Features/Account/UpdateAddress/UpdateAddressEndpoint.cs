using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Api.Users.Features.Account.UpdateAddress;

public sealed class UpdateAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPut<UpdateAddressResponse>("api/account/me/addresses/{id}", HandleAsync)
            .WithName("UpdateAddressEndpoint")
            .WithTags("account")
            .WithDescription("Updates an existing address of the authenticated user.")
            .RequireAuthorization(AuthorizationDependencyInjection.AnyAuthenticatedPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateAddressRequest request,
        [FromServices] ICommandHandler<UpdateAddressCommand, UpdateAddressResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateAddressCommand command = new(id, request);
        UpdateAddressResponse response = await handler.Handle(command, cancellationToken);

        return Results.Ok(response);
    }
}
