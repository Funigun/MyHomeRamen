using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.DeleteAddress.Models;
using MyHomeRamen.Identity.Api.Presentation;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.DeleteAddress;

public sealed class DeleteAddressEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Account";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedDelete<DeleteAddressRequest>("/me/addresses/{id}", HandleAsync)
                       .WithName("DeleteAddressEndpoint")
                       .WithDescription("Deletes an address from the authenticated user's profile.")
                       .RequireAuthorization(DependencyInjection.AnyAuthenticatedPolicy);
    }

    private static async Task<IResult> HandleAsync(
        DeleteAddressRequest id,
        [FromServices] IRequestHandler<DeleteAddressRequest, IResult> handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(id, cancellationToken);
    }
}
