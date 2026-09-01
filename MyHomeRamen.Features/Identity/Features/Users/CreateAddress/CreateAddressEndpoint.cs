using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.CreateAddress;

public sealed record CreateAddressRequest(
    string Street,
    string Building,
    string? Apartment,
    string City,
    string ZipCode,
    bool IsDefault);

public sealed record CreateAddressResponse(Guid Id);

public sealed class CreateAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateAddressResponse>("api/account/me/addresses", HandleAsync)
                       .WithName("CreateAddressEndpoint")
                       .WithTags("account")
                       .WithDescription("Adds a new address to the authenticated user's profile.")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<Results<Created<CreateAddressResponse>, BadRequest>> HandleAsync(
        [FromBody] CreateAddressRequest request,
        [FromServices] IRequestHandler<CreateAddressCommand, CreateAddressResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateAddressCommand command = new(request);
        CreateAddressResponse response = await handler.Handle(command, cancellationToken);

        return TypedResults.Created($"/api/account/me/addresses/{response.Id}", response);
    }
}
