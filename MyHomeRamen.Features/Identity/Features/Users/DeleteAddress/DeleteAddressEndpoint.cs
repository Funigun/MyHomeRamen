using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Identity.Features.Users.DeleteAddress;

public sealed class DeleteAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete("api/account/me/addresses/{id}", HandleAsync)
                       .WithName("DeleteAddressEndpoint")
                       .WithTags("account")
                       .WithDescription("Deletes an address from the authenticated user's profile.");
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<DeleteAddressCommand> handler,
        CancellationToken cancellationToken)
    {
        DeleteAddressCommand command = new(id);
        await handler.Handle(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
