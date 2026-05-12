using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Identity.Api.Features.Account.Addresses.DeleteAddress.Models;
using MyHomeRamen.Identity.Api.Presentation;

namespace MyHomeRamen.Identity.Api.Features.Account.Addresses.DeleteAddress;

public sealed class DeleteAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedDelete<DeleteAddressRequest>("api/account/me/addresses/{id}", HandleAsync)
                       .WithName("DeleteAddressEndpoint")
                       .WithTags("account")
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
