using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Api.Users.Features.Account.CreateAddress;

public sealed class CreateAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateAddressResponse>("api/account/me/addresses", HandleAsync)
                       .WithName("CreateAddressEndpoint")
                       .WithTags("account")
                       .WithDescription("Adds a new address to the authenticated user's profile.")
                       .RequireAuthorization(AuthorizationDependencyInjection.AnyAuthenticatedPolicy);
    }

    private static async Task<Results<Created<CreateAddressResponse>, BadRequest>> HandleAsync(
        [FromBody] CreateAddressRequest request,
        [FromServices] ICommandHandler<CreateAddressCommand, CreateAddressResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateAddressCommand command = new(request);
        CreateAddressResponse response = await handler.Handle(command, cancellationToken);

        return TypedResults.Created($"/api/account/me/addresses/{response.Id}", response);
    }
}
