using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Api.Users.Features.Account.CreateAddress;

public sealed class CreateAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPost<CreateAddressCommand, CreateAddressResponse>("api/account/me/addresses", HandleAsync)
                       .WithName("CreateAddressEndpoint")
                       .WithTags("account")
                       .WithDescription("Adds a new address to the authenticated user's profile.")
                       .RequireAuthorization(AuthorizationDependencyInjection.AnyAuthenticatedPolicy);
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
