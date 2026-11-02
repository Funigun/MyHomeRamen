using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.DeactivateMenuCard;

public sealed class DeactivateMenuCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete("api/menu/cards/{id:guid}", HandleAsync)
            .WithName("DeactivateMenuCardEndpoint")
            .WithTags("Menu Cards")
            .WithDescription("Deactivates a menu card.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IRequestHandler<DeactivateMenuCardCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(new DeactivateMenuCardCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
