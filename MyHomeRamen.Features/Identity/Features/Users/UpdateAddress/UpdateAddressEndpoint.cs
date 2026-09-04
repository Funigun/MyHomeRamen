using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.UpdateAddress;

public sealed record UpdateAddressRequest(
    Guid Id,
    string Street,
    string Building,
    string? Apartment,
    string City,
    string ZipCode,
    bool IsDefault);

public sealed record UpdateAddressResponse(Guid Id);

public sealed class UpdateAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPut<UpdateAddressResponse>("api/account/me/addresses/{id}", HandleAsync)
            .WithName("UpdateAddressEndpoint")
            .WithTags("account")
            .WithDescription("Updates an existing address of the authenticated user.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateAddressRequest request,
        [FromServices] IRequestHandler<UpdateAddressCommand, UpdateAddressResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateAddressCommand command = new(id, request);
        UpdateAddressResponse response = await handler.Handle(command, cancellationToken);

        return Results.Ok(response);
    }
}
